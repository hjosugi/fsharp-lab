<!-- i18n: language-switcher -->
[English](README.en.md) | [日本語](README.md)

# Parking Bounded Context Challenge

Without looking at the Subscription sample, apply the same design to Parking.

## Domain

Parking lots are registered as Drafts. When the name, address, hourly rate, and business hours are complete, they can submit a publication request. When a reviewer approves, it becomes Published. Parking lots that are Closed cannot be re-published.

## Part 1: Types

- Do not keep `ParkingId`, `HourlyRate`, `Address` as primitive types.
- Do not represent `Draft`, `PendingApproval`, `Published`, `Closed` with boolean flags.
- Avoid a design where the publication date exists within the Draft.

## Part 2: Invariants

- Name is not empty
- Rate is zero or more
- Start time is before end time
- All required fields are present

## Part 3: Deriver

`requestPublication` should be a pure function. It should not directly call the DB, clock, or logger. The necessary timestamp should be passed as an argument.

Please list the expected outcomes as discriminated unions (DU):

- `RequestAccepted`
- `AlreadyPending`
- `AlreadyPublished`
- `ParkingIsClosed`
- `MissingRequiredFields`

## Part 4: Controller

- Retrieve Parking from the repository
- Call the pure Deriver
- If accepted, save and publish a review event
- If missing, return NotFound

## Part 5: Tests

At least 10 cases. Include not only happy paths but also each union case, boundary values, and cases where effects are not called.

Start from `Starter.fsx`, and only compare with `Solution.fsx` at the end. Even if the answer differs, as long as the types follow the rules and the trade-offs are explained, it is considered correct.