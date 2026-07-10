// AND is a record. OR is a discriminated union.

type EmailAddress = private EmailAddress of string

module EmailAddress =
    let create (value: string) =
        if value.Contains "@" then
            Ok(EmailAddress value)
        else
            Error "Email address must contain @."

    let value (EmailAddress value) = value

type ContactMethod =
    | Email of EmailAddress
    | Phone of string
    | NoContact

type Customer = {
    Name: string
    ContactMethod: ContactMethod
}

let describeContact customer =
    match customer.ContactMethod with
    | Email email -> sprintf "Email %s" (EmailAddress.value email)
    | Phone phone -> sprintf "Phone %s" phone
    | NoContact -> "No contact method"

let customerResult =
    EmailAddress.create "learner@example.com"
    |> Result.map (fun email -> {
        Name = "Learner"
        ContactMethod = Email email
    })

printfn "%A" customerResult

// Output task:
// Model a parking contact method without nullable fields or boolean flags.

