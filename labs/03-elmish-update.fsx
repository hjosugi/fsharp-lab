// A dependency-free introduction to The Elm Architecture.

type RemoteData<'value, 'error> =
    | NotAsked
    | Loading
    | Loaded of 'value
    | Failed of 'error

type Parking = {
    Id: int
    Name: string
}

type Model = {
    Query: string
    Results: RemoteData<Parking list, string>
}

type Msg =
    | QueryChanged of string
    | SearchRequested
    | SearchSucceeded of Parking list
    | SearchFailed of string
    | Reset

let initialModel = {
    Query = ""
    Results = NotAsked
}

let update msg model =
    match msg with
    | QueryChanged query -> { model with Query = query }
    | SearchRequested -> { model with Results = Loading }
    | SearchSucceeded parkings -> { model with Results = Loaded parkings }
    | SearchFailed error -> { model with Results = Failed error }
    | Reset -> initialModel

let finalModel =
    initialModel
    |> update (QueryChanged "Ikebukuro")
    |> update SearchRequested
    |> update (SearchSucceeded [ { Id = 1; Name = "East Exit Parking" } ])

printfn "%A" finalModel

