namespace FSharpLab.Domain.Subscription

open System

[<RequireQualifiedAccess>]
module Upgrade =
    let private calculateProrataDays (today: DateOnly) (renewalDate: DateOnly) =
        max 0 (renewalDate.DayNumber - today.DayNumber)

    let private upgradeFee currentPlan requestedPlan =
        match currentPlan, requestedPlan with
        | PlanLevel.Basic, PlanLevel.Standard -> Money.unsafeCreate 10m
        | PlanLevel.Basic, PlanLevel.Premium -> Money.unsafeCreate 25m
        | PlanLevel.Standard, PlanLevel.Premium -> Money.unsafeCreate 15m
        | _ -> Money.unsafeCreate 0m

    let derive today requestedPlan subscription customer =
        if not (Invariants.hasNoOutstandingBalance customer) then
            UpgradeDecision.AccountOverdrawn customer.OutstandingBalance
        elif not (Invariants.isActive subscription) then
            UpgradeDecision.InvalidSubscriptionStatus subscription.Status
        elif not (Invariants.isHigherPlan requestedPlan subscription) then
            UpgradeDecision.PlanIsNotHigher(subscription.PlanLevel, requestedPlan)
        else
            let change = {
                UpdatedSubscription = {
                    subscription with
                        PlanLevel = requestedPlan
                }
                UpgradeFee = upgradeFee subscription.PlanLevel requestedPlan
                ProrataDays = calculateProrataDays today subscription.RenewsOn
            }

            UpgradeDecision.UpgradeAllowed change
