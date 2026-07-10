namespace FSharpLab.Application.Subscription

open FSharpLab.Domain.Subscription

[<RequireQualifiedAccess>]
module UpgradeSubscription =
    let create dependencies: UpgradeSubscription =
        fun request ->
            async {
                let! customerOption = dependencies.Customers.GetById request.CustomerId

                match customerOption with
                | None ->
                    return UpgradeOutcome.CustomerNotFound request.CustomerId
                | Some customer ->
                    let! subscriptionOption =
                        dependencies.Subscriptions.GetByCustomerId request.CustomerId

                    match subscriptionOption with
                    | None ->
                        return UpgradeOutcome.SubscriptionNotFound request.CustomerId
                    | Some subscription ->
                        let decision =
                            Upgrade.derive
                                (dependencies.Today())
                                request.RequestedPlan
                                subscription
                                customer

                        match decision with
                        | UpgradeDecision.AccountOverdrawn balance ->
                            do! dependencies.SendRepaymentReminder customer balance
                            return UpgradeOutcome.AccountOverdrawn balance
                        | UpgradeDecision.InvalidSubscriptionStatus status ->
                            return UpgradeOutcome.InvalidSubscriptionStatus status
                        | UpgradeDecision.PlanIsNotHigher(current, requested) ->
                            return UpgradeOutcome.PlanIsNotHigher(current, requested)
                        | UpgradeDecision.UpgradeAllowed change ->
                            let! paymentResult =
                                dependencies.Payment.Charge
                                    customer.DefaultCard
                                    change.UpgradeFee

                            match paymentResult with
                            | Error reason ->
                                return UpgradeOutcome.PaymentFailed reason
                            | Ok transactionId ->
                                do! dependencies.Subscriptions.Save change.UpdatedSubscription

                                do!
                                    dependencies.SendInvoice
                                        customer
                                        change.UpdatedSubscription
                                        change.UpgradeFee
                                        transactionId

                                return UpgradeOutcome.Succeeded(change, transactionId)
            }

