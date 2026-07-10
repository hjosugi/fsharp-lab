namespace FSharpLab.Application.Subscription

open System
open FSharpLab.Domain.Subscription

type TransactionId = private TransactionId of Guid

[<RequireQualifiedAccess>]
module TransactionId =
    let create value =
        if value = Guid.Empty then
            Error "TransactionId must not be empty."
        else
            Ok(TransactionId value)

    let value (TransactionId value) = value

type CustomerRepository = {
    GetById: CustomerId -> Async<Customer option>
}

type SubscriptionRepository = {
    GetByCustomerId: CustomerId -> Async<Subscription option>
    Save: Subscription -> Async<unit>
}

type PaymentGateway = {
    Charge: CardToken -> Money -> Async<Result<TransactionId, string>>
}

type UpgradeDependencies = {
    Customers: CustomerRepository
    Subscriptions: SubscriptionRepository
    Payment: PaymentGateway
    SendRepaymentReminder: Customer -> Money -> Async<unit>
    SendInvoice: Customer -> Subscription -> Money -> TransactionId -> Async<unit>
    Today: unit -> DateOnly
}

type UpgradeRequest = {
    CustomerId: CustomerId
    RequestedPlan: PlanLevel
}

[<RequireQualifiedAccess>]
type UpgradeOutcome =
    | Succeeded of change: UpgradeChange * transactionId: TransactionId
    | CustomerNotFound of CustomerId
    | SubscriptionNotFound of CustomerId
    | AccountOverdrawn of Money
    | InvalidSubscriptionStatus of SubscriptionStatus
    | PlanIsNotHigher of current: PlanLevel * requested: PlanLevel
    | PaymentFailed of reason: string

type UpgradeSubscription = UpgradeRequest -> Async<UpgradeOutcome>

