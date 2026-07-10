namespace FSharpLab.Infrastructure.Subscription

open System
open FSharpLab.Application.Subscription
open FSharpLab.Domain.Subscription

type ConsoleEffects = {
    Payment: PaymentGateway
    SendRepaymentReminder: Customer -> Money -> Async<unit>
    SendInvoice: Customer -> Subscription -> Money -> TransactionId -> Async<unit>
}

[<RequireQualifiedAccess>]
module ConsoleEffects =
    let create () =
        let payment: PaymentGateway = {
            Charge =
                fun card amount ->
                    async {
                        printfn
                            "Charging card %s: %.2f"
                            (CardToken.value card)
                            (Money.value amount)

                        return
                            match TransactionId.create (Guid.NewGuid()) with
                            | Ok transactionId -> Ok transactionId
                            | Error error -> invalidOp (sprintf "%A" error)
                    }
        }

        let sendRepaymentReminder customer balance =
            async {
                printfn
                    "Repayment reminder for %O: %.2f"
                    (CustomerId.value customer.Id)
                    (Money.value balance)
            }

        let sendInvoice customer subscription fee transactionId =
            async {
                printfn
                    "Invoice for %O: plan=%A fee=%.2f transaction=%O"
                    (CustomerId.value customer.Id)
                    subscription.PlanLevel
                    (Money.value fee)
                    (TransactionId.value transactionId)
            }

        {
            Payment = payment
            SendRepaymentReminder = sendRepaymentReminder
            SendInvoice = sendInvoice
        }
