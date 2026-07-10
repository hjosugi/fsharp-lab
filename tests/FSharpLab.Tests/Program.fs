module FSharpLab.Tests.Program

open System
open FSharpLab.Application.Subscription
open FSharpLab.Domain.Subscription

let mutable private failures: string list = []

let private test name action =
    try
        action ()
        printfn "PASS %s" name
    with error ->
        failures <- sprintf "%s: %s" name error.Message :: failures
        eprintfn "FAIL %s: %s" name error.Message

let private equal expected actual =
    if actual <> expected then
        failwithf "Expected %A but got %A" expected actual

let private orFail = function
    | Ok value -> value
    | Error error -> failwith error

let private customerId =
    Guid.Parse "7f018f8a-aeda-4628-aeb4-9ee6895b9114"
    |> CustomerId.create
    |> orFail

let private transactionId =
    Guid.Parse "a118264b-a3f8-4493-94a8-da3d5e1b0be2"
    |> TransactionId.create
    |> orFail

let private makeCustomer outstandingBalance = {
    Id = customerId
    OutstandingBalance = Money.create outstandingBalance |> orFail
    DefaultCard = CardToken.create "tok_test" |> orFail
}

let private makeSubscription status plan = {
    CustomerId = customerId
    PlanLevel = plan
    Status = status
    StartedOn = DateOnly(2026, 1, 1)
    RenewsOn = DateOnly(2026, 12, 31)
}

let private baseDependencies customer subscription = {
    Customers = {
        GetById = fun _ -> async { return Some customer }
    }
    Subscriptions = {
        GetByCustomerId = fun _ -> async { return Some subscription }
        Save = fun _ -> async { return () }
    }
    Payment = {
        Charge = fun _ _ -> async { return Ok transactionId }
    }
    SendRepaymentReminder = fun _ _ -> async { return () }
    SendInvoice = fun _ _ _ _ -> async { return () }
    Today = fun () -> DateOnly(2026, 7, 10)
}

[<EntryPoint>]
let main _ =
    test "Money rejects a negative amount" (fun () ->
        match Money.create -0.01m with
        | Error _ -> ()
        | Ok _ -> failwith "Negative money was accepted")

    test "Upgrade deriver returns a complete change" (fun () ->
        let customer = makeCustomer 0m

        let subscription =
            makeSubscription SubscriptionStatus.Active PlanLevel.Basic

        match Upgrade.derive (DateOnly(2026, 7, 10)) PlanLevel.Premium subscription customer with
        | UpgradeDecision.UpgradeAllowed change ->
            equal PlanLevel.Premium change.UpdatedSubscription.PlanLevel
            equal 25m (Money.value change.UpgradeFee)
            equal 174 change.ProrataDays
        | other -> failwithf "Expected an allowed upgrade but got %A" other)

    test "Outstanding balance is an explicit domain outcome" (fun () ->
        let customer = makeCustomer 20m

        let subscription =
            makeSubscription SubscriptionStatus.Active PlanLevel.Basic

        match Upgrade.derive (DateOnly(2026, 7, 10)) PlanLevel.Premium subscription customer with
        | UpgradeDecision.AccountOverdrawn balance -> equal 20m (Money.value balance)
        | other -> failwithf "Expected AccountOverdrawn but got %A" other)

    test "A suspended subscription cannot be upgraded" (fun () ->
        let customer = makeCustomer 0m

        let subscription =
            makeSubscription SubscriptionStatus.Suspended PlanLevel.Basic

        match Upgrade.derive (DateOnly(2026, 7, 10)) PlanLevel.Premium subscription customer with
        | UpgradeDecision.InvalidSubscriptionStatus SubscriptionStatus.Suspended -> ()
        | other -> failwithf "Expected InvalidSubscriptionStatus but got %A" other)

    test "A plan must be strictly higher" (fun () ->
        let customer = makeCustomer 0m

        let subscription =
            makeSubscription SubscriptionStatus.Active PlanLevel.Premium

        match Upgrade.derive (DateOnly(2026, 7, 10)) PlanLevel.Standard subscription customer with
        | UpgradeDecision.PlanIsNotHigher(PlanLevel.Premium, PlanLevel.Standard) -> ()
        | other -> failwithf "Expected PlanIsNotHigher but got %A" other)

    test "Controller sends a reminder only for the overdrawn outcome" (fun () ->
        let customer = makeCustomer 20m

        let subscription =
            makeSubscription SubscriptionStatus.Active PlanLevel.Basic

        let mutable reminderCount = 0
        let mutable paymentCount = 0
        let defaults = baseDependencies customer subscription

        let dependencies = {
            defaults with
                Payment = {
                    Charge =
                        fun _ _ ->
                            async {
                                paymentCount <- paymentCount + 1
                                return Ok transactionId
                            }
                }
                SendRepaymentReminder =
                    fun _ _ ->
                        async {
                            reminderCount <- reminderCount + 1
                        }
        }

        let outcome =
            UpgradeSubscriptionController.create dependencies {
                CustomerId = customerId
                RequestedPlan = PlanLevel.Premium
            }
            |> Async.RunSynchronously

        match outcome with
        | UpgradeOutcome.AccountOverdrawn _ -> ()
        | other -> failwithf "Expected AccountOverdrawn but got %A" other

        equal 1 reminderCount
        equal 0 paymentCount)

    test "Successful controller saves and sends one invoice" (fun () ->
        let customer = makeCustomer 0m

        let subscription =
            makeSubscription SubscriptionStatus.Active PlanLevel.Basic

        let mutable savedPlan: PlanLevel option = None
        let mutable invoiceCount = 0
        let defaults = baseDependencies customer subscription

        let dependencies = {
            defaults with
                Subscriptions = {
                    defaults.Subscriptions with
                        Save =
                            fun updated ->
                                async {
                                    savedPlan <- Some updated.PlanLevel
                                }
                }
                SendInvoice =
                    fun _ _ _ _ ->
                        async {
                            invoiceCount <- invoiceCount + 1
                        }
        }

        let outcome =
            UpgradeSubscriptionController.create dependencies {
                CustomerId = customerId
                RequestedPlan = PlanLevel.Premium
            }
            |> Async.RunSynchronously

        match outcome with
        | UpgradeOutcome.Succeeded _ -> ()
        | other -> failwithf "Expected Succeeded but got %A" other

        equal (Some PlanLevel.Premium) savedPlan
        equal 1 invoiceCount)

    test "Payment failure does not save or send an invoice" (fun () ->
        let customer = makeCustomer 0m

        let subscription =
            makeSubscription SubscriptionStatus.Active PlanLevel.Basic

        let mutable saveCount = 0
        let mutable invoiceCount = 0
        let defaults = baseDependencies customer subscription

        let dependencies = {
            defaults with
                Payment = {
                    Charge = fun _ _ -> async { return Error "declined" }
                }
                Subscriptions = {
                    defaults.Subscriptions with
                        Save =
                            fun _ ->
                                async {
                                    saveCount <- saveCount + 1
                                }
                }
                SendInvoice =
                    fun _ _ _ _ ->
                        async {
                            invoiceCount <- invoiceCount + 1
                        }
        }

        let outcome =
            UpgradeSubscriptionController.create dependencies {
                CustomerId = customerId
                RequestedPlan = PlanLevel.Premium
            }
            |> Async.RunSynchronously

        match outcome with
        | UpgradeOutcome.PaymentFailed "declined" -> ()
        | other -> failwithf "Expected PaymentFailed but got %A" other

        equal 0 saveCount
        equal 0 invoiceCount)

    if List.isEmpty failures then
        printfn "All tests passed."
        0
    else
        eprintfn "\n%d test(s) failed:" failures.Length
        failures |> List.rev |> List.iter (eprintfn "- %s")
        1
