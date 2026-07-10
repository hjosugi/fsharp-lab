type RegistrationError =
    | NameIsBlank
    | EmailIsInvalid
    | AgeIsTooLow of minimum: int

type ValidName = private ValidName of string
type ValidEmail = private ValidEmail of string

type Registration = {
    Name: ValidName
    Email: ValidEmail
    Age: int
}

let validateName (value: string) =
    if System.String.IsNullOrWhiteSpace value then
        Error NameIsBlank
    else
        Ok(ValidName value)

let validateEmail (value: string) =
    if value.Contains "@" then
        Ok(ValidEmail value)
    else
        Error EmailIsInvalid

let validateAge value =
    if value >= 18 then Ok value else Error(AgeIsTooLow 18)

let createRegistration rawName rawEmail rawAge =
    validateName rawName
    |> Result.bind (fun name ->
        validateEmail rawEmail
        |> Result.bind (fun email ->
            validateAge rawAge
            |> Result.map (fun age -> {
                Name = name
                Email = email
                Age = age
            })))

printfn "%A" (createRegistration "Learner" "learner@example.com" 20)
printfn "%A" (createRegistration "" "invalid" 16)

// Output task:
// 1. Implement bind and map yourself.
// 2. Rewrite with a result computation expression.
// 3. Change validation to accumulate every error.

