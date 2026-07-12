namespace FSharpLab.Application.Parking

open System
open FSharpLab.Domain.Parking

type ParkingRepository = {
    GetById: ParkingId -> Async<ParkingState option>
    Save: ParkingState -> Async<unit>
}

type ParkingDependencies = {
    Repository: ParkingRepository
    Publish: ParkingEvent -> Async<unit>
    Now: unit -> DateTimeOffset
}

[<RequireQualifiedAccess>]
type ParkingCommandOutcome =
    | Saved of ParkingState
    | NotFound of ParkingId
    | PublicationRequestRejected of PublicationRequestDecision
    | ApprovalRejected of ApprovalDecision
    | ClosureRejected of ClosureDecision

type RequestPublication = ParkingId -> Async<ParkingCommandOutcome>
type ApprovePublication = ParkingId -> Async<ParkingCommandOutcome>
type CloseParking = ParkingId -> Async<ParkingCommandOutcome>
