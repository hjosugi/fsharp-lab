open System

// TODO 1: Replace primitive aliases with constrained types.
type ParkingId = Guid
type HourlyRate = decimal

// TODO 2: Model the lifecycle without boolean flags.
type ParkingState =
    | Draft
    | PendingApproval
    | Published
    | Closed

type Parking = {
    Id: ParkingId
    Name: string
    HourlyRate: HourlyRate option
    State: ParkingState
}

// TODO 3: List every expected outcome as a discriminated union.
type RequestPublicationOutcome =
    | NotImplementedYet

// TODO 4: Implement this as a pure function.
let requestPublication now parking =
    let _ = now, parking
    NotImplementedYet

printfn "Start by changing the types, then implement the workflow."

