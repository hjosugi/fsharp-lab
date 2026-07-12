namespace FSharpLab.Application.Parking

open FSharpLab.Domain.Parking

[<RequireQualifiedAccess>]
module ParkingController =
    let private load dependencies parkingId action =
        async {
            let! state = dependencies.Repository.GetById parkingId

            match state with
            | None -> return ParkingCommandOutcome.NotFound parkingId
            | Some current -> return! action current
        }

    let requestPublication dependencies: RequestPublication =
        fun parkingId ->
            load dependencies parkingId (fun current ->
                async {
                    match ParkingWorkflow.requestPublication (dependencies.Now()) current with
                    | PublicationRequestDecision.Accepted pending ->
                        let next = ParkingState.PendingApproval pending
                        do! dependencies.Repository.Save next
                        do! dependencies.Publish(PublicationRequested pending)
                        return ParkingCommandOutcome.Saved next
                    | rejected ->
                        return ParkingCommandOutcome.PublicationRequestRejected rejected
                })

    let approvePublication dependencies: ApprovePublication =
        fun parkingId ->
            load dependencies parkingId (fun current ->
                async {
                    match ParkingWorkflow.approvePublication (dependencies.Now()) current with
                    | ApprovalDecision.Approved published ->
                        let next = ParkingState.Published published
                        do! dependencies.Repository.Save next
                        do! dependencies.Publish(PublicationApproved published)
                        return ParkingCommandOutcome.Saved next
                    | rejected -> return ParkingCommandOutcome.ApprovalRejected rejected
                })

    let close dependencies: CloseParking =
        fun parkingId ->
            load dependencies parkingId (fun current ->
                async {
                    match ParkingWorkflow.close (dependencies.Now()) current with
                    | ClosureDecision.Closed closed ->
                        let next = ParkingState.Closed closed
                        do! dependencies.Repository.Save next
                        do! dependencies.Publish(ParkingClosed closed)
                        return ParkingCommandOutcome.Saved next
                    | rejected -> return ParkingCommandOutcome.ClosureRejected rejected
                })
