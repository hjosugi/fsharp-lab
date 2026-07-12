namespace FSharpLab.Infrastructure.Parking

open System.Collections.Generic
open FSharpLab.Application.Parking
open FSharpLab.Domain.Parking

type InMemoryParkingAdapters = {
    Repository: ParkingRepository
    Events: ParkingEvent ResizeArray
}

[<RequireQualifiedAccess>]
module InMemoryParking =
    let private idOf = function
        | ParkingState.Draft draft -> draft.Id
        | ParkingState.PendingApproval pending -> pending.Parking.Id
        | ParkingState.Published published -> published.Parking.Id
        | ParkingState.Closed closed -> closed.Id

    let create initialStates =
        let store = Dictionary<ParkingId, ParkingState>()
        let events = ResizeArray<ParkingEvent>()

        initialStates |> Seq.iter (fun state -> store[idOf state] <- state)

        let repository = {
            GetById =
                fun parkingId ->
                    async {
                        match store.TryGetValue parkingId with
                        | true, state -> return Some state
                        | false, _ -> return None
                    }
            Save =
                fun state ->
                    async {
                        store[idOf state] <- state
                    }
        }

        {
            Repository = repository
            Events = events
        }

    let publisher adapters event =
        async { adapters.Events.Add event }
