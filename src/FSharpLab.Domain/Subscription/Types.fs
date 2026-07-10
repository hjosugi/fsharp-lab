namespace FSharpLab.Domain.Subscription

open System

type CustomerId = private CustomerId of Guid

[<RequireQualifiedAccess>]
module CustomerId =
    let create value =
        if value = Guid.Empty then
            Error "CustomerId must not be empty."
        else
            Ok(CustomerId value)

    let parse (text: string) =
        match Guid.TryParse text with
        | true, value -> create value
        | false, _ -> Error "CustomerId must be a valid GUID."

    let value (CustomerId value) = value

type Money = private Money of decimal

[<RequireQualifiedAccess>]
module Money =
    let create value =
        if value < 0m then
            Error "Money must not be negative."
        else
            Ok(Money value)

    let internal unsafeCreate value = Money value
    let value (Money value) = value
    let isZero money = value money = 0m

type CardToken = private CardToken of string

[<RequireQualifiedAccess>]
module CardToken =
    let create (value: string) =
        if String.IsNullOrWhiteSpace value then
            Error "CardToken must not be blank."
        else
            Ok(CardToken value)

    let value (CardToken value) = value

[<RequireQualifiedAccess>]
type PlanLevel =
    | Basic
    | Standard
    | Premium

[<RequireQualifiedAccess>]
module PlanLevel =
    let rank = function
        | PlanLevel.Basic -> 0
        | PlanLevel.Standard -> 1
        | PlanLevel.Premium -> 2

[<RequireQualifiedAccess>]
type SubscriptionStatus =
    | Active
    | Suspended
    | Cancelled

type Customer = {
    Id: CustomerId
    OutstandingBalance: Money
    DefaultCard: CardToken
}

type Subscription = {
    CustomerId: CustomerId
    PlanLevel: PlanLevel
    Status: SubscriptionStatus
    StartedOn: DateOnly
    RenewsOn: DateOnly
}

type UpgradeChange = {
    UpdatedSubscription: Subscription
    UpgradeFee: Money
    ProrataDays: int
}

[<RequireQualifiedAccess>]
type UpgradeDecision =
    | UpgradeAllowed of UpgradeChange
    | AccountOverdrawn of balanceOwing: Money
    | InvalidSubscriptionStatus of current: SubscriptionStatus
    | PlanIsNotHigher of current: PlanLevel * requested: PlanLevel

