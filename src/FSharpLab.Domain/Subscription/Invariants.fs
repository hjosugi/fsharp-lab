namespace FSharpLab.Domain.Subscription

[<RequireQualifiedAccess>]
module Invariants =
    let hasNoOutstandingBalance customer =
        Money.isZero customer.OutstandingBalance

    let isActive subscription =
        subscription.Status = SubscriptionStatus.Active

    let isHigherPlan requested subscription =
        PlanLevel.rank requested > PlanLevel.rank subscription.PlanLevel

