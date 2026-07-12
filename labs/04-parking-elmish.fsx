open System

// Protocol types intentionally differ from the Parking domain aggregate.
// A client receives only the fields needed by this screen.
type ParkingSummaryDto = {
    Id: Guid
    Name: string
    Address: string
    HourlyRate: decimal
    IsPublished: bool
}

type SearchQueryDto = { Text: string }

[<RequireQualifiedAccess>]
type SearchErrorDto =
    | QueryTooShort
    | NotAuthorized

[<RequireQualifiedAccess>]
type PublicationErrorDto =
    | NotFound
    | MissingRequiredFields of string list
    | AlreadyPending
    | AlreadyPublished
    | NotAuthorized

type ParkingApi = {
    Search: SearchQueryDto -> Async<Result<ParkingSummaryDto list, SearchErrorDto>>
    RequestPublication: Guid -> Async<Result<unit, PublicationErrorDto>>
}

// Network/system failures are separate from typed business outcomes.
[<RequireQualifiedAccess>]
type RemoteError<'businessError> =
    | Business of 'businessError
    | Network of message: string

type RemoteData<'value, 'error> =
    | NotAsked
    | Loading
    | Loaded of 'value
    | Failed of 'error

type Model = {
    Query: string
    Search: RemoteData<ParkingSummaryDto list, RemoteError<SearchErrorDto>>
    Publication: Map<Guid, RemoteData<unit, RemoteError<PublicationErrorDto>>>
}

type Msg =
    | QueryChanged of string
    | SearchSubmitted
    | SearchCompleted of Result<ParkingSummaryDto list, RemoteError<SearchErrorDto>>
    | PublicationRequested of Guid
    | PublicationCompleted of Guid * Result<unit, RemoteError<PublicationErrorDto>>
    | Reset

// Keeping commands as data makes update pure and independently testable.
type Effect = ParkingApi -> Async<Msg>

let initialModel = {
    Query = ""
    Search = NotAsked
    Publication = Map.empty
}

let private protect mapResult operation =
    async {
        try
            let! result = operation
            return result |> Result.mapError (RemoteError.Business >> mapResult)
        with error ->
            return Error(RemoteError.Network error.Message |> mapResult)
    }

let private searchEffect query: Effect =
    fun api ->
        async {
            let! result =
                protect id (api.Search { Text = query })

            return SearchCompleted result
        }

let private publicationEffect parkingId: Effect =
    fun api ->
        async {
            let! result =
                protect id (api.RequestPublication parkingId)

            return PublicationCompleted(parkingId, result)
        }

let update msg model =
    match msg with
    | QueryChanged query -> { model with Query = query }, []
    | SearchSubmitted ->
        { model with Search = Loading }, [ searchEffect model.Query ]
    | SearchCompleted(Ok parkings) ->
        { model with Search = Loaded parkings }, []
    | SearchCompleted(Error error) ->
        { model with Search = Failed error }, []
    | PublicationRequested parkingId ->
        {
            model with
                Publication = model.Publication.Add(parkingId, Loading)
        }, [ publicationEffect parkingId ]
    | PublicationCompleted(parkingId, Ok()) ->
        {
            model with
                Publication = model.Publication.Add(parkingId, Loaded())
        }, []
    | PublicationCompleted(parkingId, Error error) ->
        {
            model with
                Publication = model.Publication.Add(parkingId, Failed error)
        }, []
    | Reset -> initialModel, []

let runEffects api effects =
    effects |> List.map (fun effect -> effect api |> Async.RunSynchronously)

// Fake adapter: the UI and tests use the same record-of-async-functions contract
// as a Fable.Remoting client, without importing server/domain types.
let parkingId = Guid.Parse "950aeb64-8d30-4387-bcb8-96cb45878087"

let fakeApi = {
    Search =
        fun query ->
            async {
                if query.Text.Trim().Length < 2 then
                    return Error SearchErrorDto.QueryTooShort
                else
                    return
                        Ok [
                            {
                                Id = parkingId
                                Name = "Ikebukuro East"
                                Address = "1-1 Ikebukuro, Tokyo"
                                HourlyRate = 400m
                                IsPublished = false
                            }
                        ]
            }
    RequestPublication =
        fun requestedId ->
            async {
                return
                    if requestedId = parkingId then Ok()
                    else Error PublicationErrorDto.NotFound
            }
}

let equal expected actual =
    if expected <> actual then failwithf "Expected %A but got %A" expected actual

// End-to-end Elmish loop: event -> pure update -> typed protocol effect -> Msg -> update.
let queried, _ = update (QueryChanged "Ikebukuro") initialModel
let loading, searchCommands = update SearchSubmitted queried
equal Loading loading.Search

let searchMessage = runEffects fakeApi searchCommands |> List.exactlyOne
let searched, _ = update searchMessage loading

let result =
    match searched.Search with
    | Loaded [ parking ] -> parking
    | other -> failwithf "Expected one loaded parking, got %A" other

let publishing, publicationCommands = update (PublicationRequested result.Id) searched
equal (Some Loading) (publishing.Publication.TryFind result.Id)

let publicationMessage = runEffects fakeApi publicationCommands |> List.exactlyOne
let published, _ = update publicationMessage publishing
equal (Some(Loaded())) (published.Publication.TryFind result.Id)

printfn "Parking Elmish flow passed: %s" result.Name
