module FSharpLab.Cli.Program

open System
open FSharpLab.Application.Subscription
open FSharpLab.Domain.Subscription
open FSharpLab.Infrastructure.Subscription

let private orFail = function
    | Ok value -> value
    | Error error -> invalidOp error

[<EntryPoint>]
let main _ =
    let customerId =
        Guid.Parse "f3ce8ce0-323b-4b77-85f5-fc193114b8e8"
        |> CustomerId.create
        |> orFail

    let customer = {
        Id = customerId
        OutstandingBalance = Money.create 0m |> orFail
        DefaultCard = CardToken.create "tok_learning_only" |> orFail
    }

    let subscription = {
        CustomerId = customerId
        PlanLevel = PlanLevel.Basic
        Status = SubscriptionStatus.Active
        StartedOn = DateOnly(2026, 1, 1)
        RenewsOn = DateOnly(2026, 12, 31)
    }

    let stores = InMemory.create [ customer ] [ subscription ]
    let effects = ConsoleEffects.create ()

    let dependencies = {
        Customers = stores.Customers
        Subscriptions = stores.Subscriptions
        Payment = effects.Payment
        SendRepaymentReminder = effects.SendRepaymentReminder
        SendInvoice = effects.SendInvoice
        Today = fun () -> DateOnly(2026, 7, 10)
    }

    let upgrade = UpgradeSubscriptionController.create dependencies

    let outcome =
        upgrade {
            CustomerId = customerId
            RequestedPlan = PlanLevel.Premium
        }
        |> Async.RunSynchronously

    match outcome with
    | UpgradeOutcome.Succeeded(change, transactionId) ->
        printfn
            "Upgrade succeeded: plan=%A fee=%.2f days=%d transaction=%O"
            change.UpdatedSubscription.PlanLevel
            (Money.value change.UpgradeFee)
            change.ProrataDays
            (TransactionId.value transactionId)
        0
    | other ->
        eprintfn "Upgrade did not complete: %A" other
        1
