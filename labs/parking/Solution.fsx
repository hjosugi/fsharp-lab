open System

type ParkingId = private ParkingId of Guid

module ParkingId =
    let create value =
        if value = Guid.Empty then Error "ParkingId is empty."
        else Ok(ParkingId value)

type HourlyRate = private HourlyRate of decimal

module HourlyRate =
    let create value =
        if value < 0m then Error "HourlyRate is negative."
        else Ok(HourlyRate value)

type Address = private Address of string

module Address =
    let create (value: string) =
        if String.IsNullOrWhiteSpace value then Error "Address is blank."
        else Ok(Address value)

type DraftParking = {
    Id: ParkingId
    Name: string
    Address: Address option
    HourlyRate: HourlyRate option
}

type CompletedParking = {
    Id: ParkingId
    Name: string
    Address: Address
    HourlyRate: HourlyRate
}

type PendingParking = {
    Parking: CompletedParking
    RequestedAt: DateTimeOffset
}

type PublishedParking = {
    Parking: CompletedParking
    PublishedAt: DateTimeOffset
}

type ClosedParking = {
    Id: ParkingId
    ClosedAt: DateTimeOffset
}

type ParkingState =
    | Draft of DraftParking
    | PendingApproval of PendingParking
    | Published of PublishedParking
    | Closed of ClosedParking

[<RequireQualifiedAccess>]
type MissingField =
    | Name
    | Address
    | HourlyRate

type RequestPublicationOutcome =
    | RequestAccepted of PendingParking
    | MissingRequiredFields of MissingField list
    | AlreadyPending
    | AlreadyPublished
    | ParkingIsClosed

let private missingFields draft =
    [
        if String.IsNullOrWhiteSpace draft.Name then MissingField.Name
        if Option.isNone draft.Address then MissingField.Address
        if Option.isNone draft.HourlyRate then MissingField.HourlyRate
    ]

let requestPublication now parkingState =
    match parkingState with
    | PendingApproval _ -> AlreadyPending
    | Published _ -> AlreadyPublished
    | Closed _ -> ParkingIsClosed
    | Draft draft ->
        match draft.Address, draft.HourlyRate, missingFields draft with
        | Some address, Some hourlyRate, [] ->
            let completed = {
                Id = draft.Id
                Name = draft.Name
                Address = address
                HourlyRate = hourlyRate
            }

            RequestAccepted {
                Parking = completed
                RequestedAt = now
            }
        | _, _, missing -> MissingRequiredFields missing

let orFail = function
    | Ok value -> value
    | Error error -> failwith error

let draft =
    Draft {
        Id = ParkingId.create (Guid.NewGuid()) |> orFail
        Name = "Ikebukuro East Parking"
        Address = Address.create "1-1 Ikebukuro, Tokyo" |> Result.map Some |> orFail
        HourlyRate = HourlyRate.create 400m |> Result.map Some |> orFail
    }

let outcome = requestPublication DateTimeOffset.UtcNow draft
printfn "%A" outcome
