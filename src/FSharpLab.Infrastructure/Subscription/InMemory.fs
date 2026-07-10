namespace FSharpLab.Infrastructure.Subscription

open System.Collections.Generic
open FSharpLab.Application.Subscription
open FSharpLab.Domain.Subscription

type InMemoryAdapters = {
    Customers: CustomerRepository
    Subscriptions: SubscriptionRepository
}

[<RequireQualifiedAccess>]
module InMemory =
    let create customers subscriptions =
        let customerStore = Dictionary<CustomerId, Customer>()
        let subscriptionStore = Dictionary<CustomerId, Subscription>()

        customers
        |> Seq.iter (fun customer -> customerStore[customer.Id] <- customer)

        subscriptions
        |> Seq.iter (fun subscription ->
            subscriptionStore[subscription.CustomerId] <- subscription)

        let customerRepository: CustomerRepository = {
            GetById =
                fun customerId ->
                    async {
                        match customerStore.TryGetValue customerId with
                        | true, customer -> return Some customer
                        | false, _ -> return None
                    }
        }

        let subscriptionRepository: SubscriptionRepository = {
            GetByCustomerId =
                fun customerId ->
                    async {
                        match subscriptionStore.TryGetValue customerId with
                        | true, subscription -> return Some subscription
                        | false, _ -> return None
                    }
            Save =
                fun subscription ->
                    async {
                        subscriptionStore[subscription.CustomerId] <- subscription
                    }
        }

        {
            Customers = customerRepository
            Subscriptions = subscriptionRepository
        }

