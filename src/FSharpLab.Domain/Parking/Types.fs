namespace FSharpLab.Domain.Parking

open System

[<RequireQualifiedAccess>]
type ParkingIdError =
    | Empty

type ParkingId = private ParkingId of Guid

[<RequireQualifiedAccess>]
module ParkingId =
    let create value =
        if value = Guid.Empty then Error ParkingIdError.Empty
        else Ok(ParkingId value)

    let value (ParkingId value) = value

[<RequireQualifiedAccess>]
type ParkingNameError =
    | Blank
    | TooLong of actualLength: int

type ParkingName = private ParkingName of string

[<RequireQualifiedAccess>]
module ParkingName =
    let create (value: string) =
        if String.IsNullOrWhiteSpace value then
            Error ParkingNameError.Blank
        elif value.Trim().Length > 120 then
            Error(ParkingNameError.TooLong(value.Trim().Length))
        else
            Ok(ParkingName(value.Trim()))

    let value (ParkingName value) = value

[<RequireQualifiedAccess>]
type AddressError =
    | Blank

type Address = private Address of string

[<RequireQualifiedAccess>]
module Address =
    let create (value: string) =
        if String.IsNullOrWhiteSpace value then Error AddressError.Blank
        else Ok(Address(value.Trim()))

    let value (Address value) = value

[<RequireQualifiedAccess>]
type HourlyRateError =
    | Negative of value: decimal

type HourlyRate = private HourlyRate of decimal

[<RequireQualifiedAccess>]
module HourlyRate =
    let create value =
        if value < 0m then Error(HourlyRateError.Negative value)
        else Ok(HourlyRate value)

    let value (HourlyRate value) = value

[<RequireQualifiedAccess>]
type OpeningHoursError =
    | StartMustBeBeforeEnd

type OpeningHours = private {
    OpensAt: TimeOnly
    ClosesAt: TimeOnly
}

[<RequireQualifiedAccess>]
module OpeningHours =
    let create opensAt closesAt =
        if opensAt >= closesAt then Error OpeningHoursError.StartMustBeBeforeEnd
        else Ok { OpensAt = opensAt; ClosesAt = closesAt }

    let opensAt hours = hours.OpensAt
    let closesAt hours = hours.ClosesAt

type DraftParking = {
    Id: ParkingId
    Name: ParkingName
    Address: Address option
    HourlyRate: HourlyRate option
    OpeningHours: OpeningHours option
}

type CompletedParking = {
    Id: ParkingId
    Name: ParkingName
    Address: Address
    HourlyRate: HourlyRate
    OpeningHours: OpeningHours
}

type PendingParking = {
    Parking: CompletedParking
    RequestedAt: DateTimeOffset
}

type PublishedParking = {
    Parking: CompletedParking
    RequestedAt: DateTimeOffset
    PublishedAt: DateTimeOffset
}

type ClosedParking = {
    Id: ParkingId
    ClosedAt: DateTimeOffset
    PreviousPublication: PublishedParking option
}

type ParkingState =
    | Draft of DraftParking
    | PendingApproval of PendingParking
    | Published of PublishedParking
    | Closed of ClosedParking

[<RequireQualifiedAccess>]
type MissingField =
    | Address
    | HourlyRate
    | OpeningHours

[<RequireQualifiedAccess>]
type PublicationRequestDecision =
    | Accepted of PendingParking
    | MissingRequiredFields of MissingField list
    | AlreadyPending
    | AlreadyPublished
    | ParkingIsClosed

[<RequireQualifiedAccess>]
type ApprovalDecision =
    | Approved of PublishedParking
    | NotPending
    | ParkingIsClosed

[<RequireQualifiedAccess>]
type ClosureDecision =
    | Closed of ClosedParking
    | AlreadyClosed

type ParkingEvent =
    | PublicationRequested of PendingParking
    | PublicationApproved of PublishedParking
    | ParkingClosed of ClosedParking
