namespace FSharpLab.Domain.Parking

[<RequireQualifiedAccess>]
module ParkingWorkflow =
    let requestPublication now state =
        match state with
        | PendingApproval _ -> PublicationRequestDecision.AlreadyPending
        | Published _ -> PublicationRequestDecision.AlreadyPublished
        | Closed _ -> PublicationRequestDecision.ParkingIsClosed
        | Draft draft ->
            let missing =
                [
                    if draft.Address.IsNone then MissingField.Address
                    if draft.HourlyRate.IsNone then MissingField.HourlyRate
                    if draft.OpeningHours.IsNone then MissingField.OpeningHours
                ]

            match draft.Address, draft.HourlyRate, draft.OpeningHours, missing with
            | Some address, Some hourlyRate, Some openingHours, [] ->
                PublicationRequestDecision.Accepted {
                    Parking = {
                        Id = draft.Id
                        Name = draft.Name
                        Address = address
                        HourlyRate = hourlyRate
                        OpeningHours = openingHours
                    }
                    RequestedAt = now
                }
            | _ -> PublicationRequestDecision.MissingRequiredFields missing

    let approvePublication now state =
        match state with
        | PendingApproval pending ->
            ApprovalDecision.Approved {
                Parking = pending.Parking
                RequestedAt = pending.RequestedAt
                PublishedAt = now
            }
        | Closed _ -> ApprovalDecision.ParkingIsClosed
        | Draft _
        | Published _ -> ApprovalDecision.NotPending

    let close now state =
        match state with
        | Closed _ -> ClosureDecision.AlreadyClosed
        | Draft draft ->
            ClosureDecision.Closed {
                Id = draft.Id
                ClosedAt = now
                PreviousPublication = None
            }
        | PendingApproval pending ->
            ClosureDecision.Closed {
                Id = pending.Parking.Id
                ClosedAt = now
                PreviousPublication = None
            }
        | Published published ->
            ClosureDecision.Closed {
                Id = published.Parking.Id
                ClosedAt = now
                PreviousPublication = Some published
            }
