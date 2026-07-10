namespace FSharpLab.Domain.Subscription

open System

[<RequireQualifiedAccess>]
type CustomerIdError =
    | Empty
    | InvalidFormat of input: string

type CustomerId = private CustomerId of Guid

[<RequireQualifiedAccess>]
module CustomerId =
    let create value =
        if value = Guid.Empty then
            Error CustomerIdError.Empty
        else
            Ok(CustomerId value)

    let parse (text: string) =
        match Guid.TryParse text with
        | true, value -> create value
        | false, _ -> Error(CustomerIdError.InvalidFormat text)

    let value (CustomerId value) = value

[<RequireQualifiedAccess>]
type MoneyError =
    | NegativeAmount of value: decimal

type Money = private Money of decimal

[<RequireQualifiedAccess>]
module Money =
    let create value =
        if value < 0m then
            Error(MoneyError.NegativeAmount value)
        else
            Ok(Money value)

    let internal unsafeCreate value = Money value
    let value (Money value) = value
    let isZero money = value money = 0m

[<RequireQualifiedAccess>]
type CardTokenError =
    | Blank

type CardToken = private CardToken of string

[<RequireQualifiedAccess>]
module CardToken =
    let create (value: string) =
        if String.IsNullOrWhiteSpace value then
            Error CardTokenError.Blank
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
