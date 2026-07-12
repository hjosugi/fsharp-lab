module FSharpLab.Tests.ParkingTests

open System
open FSharpLab.Application.Parking
open FSharpLab.Domain.Parking
open FSharpLab.Infrastructure.Parking

let private equal expected actual =
    if actual <> expected then failwithf "Expected %A but got %A" expected actual

let private orFail = function
    | Ok value -> value
    | Error error -> failwithf "Unexpected validation error: %A" error

let private parkingId =
    Guid.Parse "f81708d4-cd34-47f0-84b5-6faacda5ef51"
    |> ParkingId.create
    |> orFail

let private now = DateTimeOffset(2026, 7, 12, 9, 0, 0, TimeSpan.Zero)

let private completeDraft () =
    ParkingState.Draft {
        Id = parkingId
        Name = ParkingName.create "Ikebukuro East" |> orFail
        Address = Address.create "1-1 Ikebukuro, Tokyo" |> Result.map Some |> orFail
        HourlyRate = HourlyRate.create 400m |> Result.map Some |> orFail
        OpeningHours =
            OpeningHours.create (TimeOnly(8, 0)) (TimeOnly(22, 0))
            |> Result.map Some
            |> orFail
    }

let private pending () =
    match ParkingWorkflow.requestPublication now (completeDraft ()) with
    | PublicationRequestDecision.Accepted value -> ParkingState.PendingApproval value
    | other -> failwithf "Expected accepted draft: %A" other

let private published () =
    match ParkingWorkflow.approvePublication (now.AddHours 1) (pending ()) with
    | ApprovalDecision.Approved value -> ParkingState.Published value
    | other -> failwithf "Expected approved parking: %A" other

let private test name action =
    action ()
    printfn "PASS Parking: %s" name

let run () =
    test "ParkingId rejects an empty GUID" (fun () ->
        equal (Error ParkingIdError.Empty) (ParkingId.create Guid.Empty))

    test "ParkingName trims input and rejects blanks" (fun () ->
        equal (Error ParkingNameError.Blank) (ParkingName.create "  ")
        equal "North Lot" (ParkingName.create "  North Lot  " |> orFail |> ParkingName.value))

    test "HourlyRate accepts zero and rejects negatives" (fun () ->
        equal 0m (HourlyRate.create 0m |> orFail |> HourlyRate.value)

        match HourlyRate.create -1m with
        | Error(HourlyRateError.Negative -1m) -> ()
        | other -> failwithf "Expected typed negative-rate error: %A" other)

    test "OpeningHours requires start before end" (fun () ->
        equal
            (Error OpeningHoursError.StartMustBeBeforeEnd)
            (OpeningHours.create (TimeOnly(20, 0)) (TimeOnly(8, 0))))

    test "Publication request reports every missing field" (fun () ->
        let incomplete =
            ParkingState.Draft {
                Id = parkingId
                Name = ParkingName.create "Incomplete" |> orFail
                Address = None
                HourlyRate = None
                OpeningHours = None
            }

        match ParkingWorkflow.requestPublication now incomplete with
        | PublicationRequestDecision.MissingRequiredFields fields ->
            equal
                [ MissingField.Address; MissingField.HourlyRate; MissingField.OpeningHours ]
                fields
        | other -> failwithf "Expected missing fields: %A" other)

    test "Complete draft becomes pending with supplied clock" (fun () ->
        match ParkingWorkflow.requestPublication now (completeDraft ()) with
        | PublicationRequestDecision.Accepted request ->
            equal now request.RequestedAt
            equal parkingId request.Parking.Id
        | other -> failwithf "Expected accepted request: %A" other)

    test "Pending parking cannot request publication twice" (fun () ->
        equal
            PublicationRequestDecision.AlreadyPending
            (ParkingWorkflow.requestPublication now (pending ())))

    test "Only pending parking can be approved" (fun () ->
        equal
            ApprovalDecision.NotPending
            (ParkingWorkflow.approvePublication now (completeDraft ())))

    test "Approval preserves request time and records publication time" (fun () ->
        match ParkingWorkflow.approvePublication (now.AddHours 1) (pending ()) with
        | ApprovalDecision.Approved value ->
            equal now value.RequestedAt
            equal (now.AddHours 1) value.PublishedAt
        | other -> failwithf "Expected approval: %A" other)

    test "Closing a published parking retains publication history" (fun () ->
        match ParkingWorkflow.close (now.AddDays 1) (published ()) with
        | ClosureDecision.Closed value ->
            equal parkingId value.Id
            if value.PreviousPublication.IsNone then failwith "Publication history was lost"
        | other -> failwithf "Expected closure: %A" other)

    test "Closed parking cannot transition again" (fun () ->
        let closed =
            match ParkingWorkflow.close now (completeDraft ()) with
            | ClosureDecision.Closed value -> ParkingState.Closed value
            | other -> failwithf "Expected closure: %A" other

        equal ClosureDecision.AlreadyClosed (ParkingWorkflow.close now closed)
        equal ApprovalDecision.ParkingIsClosed (ParkingWorkflow.approvePublication now closed)
        equal PublicationRequestDecision.ParkingIsClosed (ParkingWorkflow.requestPublication now closed))

    test "Request controller saves and publishes exactly once" (fun () ->
        let adapters = InMemoryParking.create [ completeDraft () ]
        let dependencies = {
            Repository = adapters.Repository
            Publish = InMemoryParking.publisher adapters
            Now = fun () -> now
        }

        match ParkingController.requestPublication dependencies parkingId |> Async.RunSynchronously with
        | ParkingCommandOutcome.Saved(ParkingState.PendingApproval _) -> ()
        | other -> failwithf "Expected saved pending parking: %A" other

        equal 1 adapters.Events.Count

        match adapters.Events[0] with
        | PublicationRequested _ -> ()
        | other -> failwithf "Expected PublicationRequested: %A" other)

    test "Rejected command performs no save or publish effect" (fun () ->
        let mutable saves = 0
        let mutable events = 0
        let state = pending ()
        let dependencies = {
            Repository = {
                GetById = fun _ -> async { return Some state }
                Save = fun _ -> async { saves <- saves + 1 }
            }
            Publish = fun _ -> async { events <- events + 1 }
            Now = fun () -> now
        }

        match ParkingController.requestPublication dependencies parkingId |> Async.RunSynchronously with
        | ParkingCommandOutcome.PublicationRequestRejected PublicationRequestDecision.AlreadyPending -> ()
        | other -> failwithf "Expected rejection: %A" other

        equal 0 saves
        equal 0 events)

    test "Missing aggregate stops before clock, save, and publish" (fun () ->
        let mutable clocks = 0
        let mutable saves = 0
        let mutable events = 0
        let dependencies = {
            Repository = {
                GetById = fun _ -> async { return None }
                Save = fun _ -> async { saves <- saves + 1 }
            }
            Publish = fun _ -> async { events <- events + 1 }
            Now = fun () -> clocks <- clocks + 1; now
        }

        match ParkingController.close dependencies parkingId |> Async.RunSynchronously with
        | ParkingCommandOutcome.NotFound missing -> equal parkingId missing
        | other -> failwithf "Expected NotFound: %A" other

        equal 0 clocks
        equal 0 saves
        equal 0 events)

    test "End-to-end request, approval, and closure persist all transitions" (fun () ->
        let adapters = InMemoryParking.create [ completeDraft () ]
        let mutable clock = now
        let dependencies = {
            Repository = adapters.Repository
            Publish = InMemoryParking.publisher adapters
            Now = fun () -> clock
        }

        ParkingController.requestPublication dependencies parkingId
        |> Async.RunSynchronously
        |> ignore

        clock <- now.AddHours 1
        ParkingController.approvePublication dependencies parkingId
        |> Async.RunSynchronously
        |> ignore

        clock <- now.AddDays 1

        match ParkingController.close dependencies parkingId |> Async.RunSynchronously with
        | ParkingCommandOutcome.Saved(ParkingState.Closed closed) ->
            equal clock closed.ClosedAt
            if closed.PreviousPublication.IsNone then failwith "Published state was not retained"
        | other -> failwithf "Expected saved closure: %A" other

        equal 3 adapters.Events.Count)
