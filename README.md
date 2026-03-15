# Payment Gateway Challenge

This repository contains a pragmatic implementation of the payment-gateway exercise focused on the functional requirements, clear validation boundaries, and maintainable tests.

## Summary

The solution exposes:

- `POST /api/payments` to process a payment
- `GET /api/payments/{id}` to retrieve a previously processed payment

The implementation keeps request validation, domain rules, persistence, and external bank communication separated without adding unnecessary abstraction.

## Requirements Mapping

### Processing a payment

The API exposes `POST /api/payments` to process a card payment request.

### Request validation

`PostPaymentRequest` performs fail-fast validation for input shape and obviously invalid values:

- `CardNumber`
  - required
  - numeric only
  - length `14-19`
- `ExpiryMonth`
  - required
  - must be between `1-12`
- `ExpiryYear`
  - required
  - validated against a realistic dynamic range from the current year to the current year plus 20 to reject nonsensical inputs early
- `Currency`
  - required
  - exactly `3` characters
- `Amount`
  - required
  - integer
  - must be greater than `0`
- `CVV`
  - required
  - numeric only
  - length `3-4`

### Business rules

Business rules are enforced in the domain model via `Payment.Create(...)`:

- Expiry validation follows standard card-processing behaviour:
  - a card is valid through the end of its expiry month
  - the current expiry month is accepted
  - any prior month is rejected
- Supported currencies are intentionally limited to `GBP`, `USD`, and `EUR`
  - this matches the exercise instruction to validate against no more than 3 ISO currency codes

### Card data handling

The full card number is used only for the acquiring bank request and is not persisted.
Only the last 4 digits are stored and returned, which aligns with the exercise requirement.

### Storage

The exercise does not require a real database, so the solution uses an in-memory store behind `IPaymentsRepository`.
Processed payments are stored as internal `Payment` domain objects rather than API response DTOs.

### Error handling

- Invalid request or failed business validation returns `201 Created` with `Status = Rejected`
- Acquiring bank unavailability returns `502 Bad Gateway`

For rejected requests, the acquiring bank is not contacted and no payment is stored.

### Testing

The solution includes:

- unit tests for `PostPaymentRequest` input validation
- unit tests for `Payment.Create(...)` business rules
- integration tests for the `PaymentsController`

This keeps the tests focused on the parts of the code that contain actual behavior:

- request-shape validation at the API boundary
- payment business rules in the domain
- end-to-end API behavior and response contracts

I deliberately did not add low-value tests for simple DTOs or thin storage wrappers.

### Validation approach

The validation split follows Microsoft guidance for separating API model validation from domain invariants:

- ASP.NET Core model validation is used at the request boundary for fail-fast input validation:
  https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation?view=aspnetcore-9.0
- `IValidatableObject` is used for dynamic request-level checks that depend on the current date:
  https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.ivalidatableobject?view=net-9.0
- Domain invariants are enforced in the domain model, following Microsoft guidance on keeping business validation in the domain layer:
  https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/domain-model-layer-validations

## Assumptions and Design Decisions

- `Amount` is represented as an `int` because the contract uses minor currency units rather than decimal major-unit amounts.
- `ExpiryYear` uses a dynamic sanity range at the request boundary to fail fast on unrealistic values, while the actual card-expiry decision is handled in the domain model.
- Card expiry follows normal banking behavior: a card remains valid through the end of its expiry month.
- Invalid payment requests return `201 Created` with `Status = Rejected`, do not contact the acquiring bank, and do not create a stored payment.
- Successful calls to the acquiring bank return a payment response with status `Authorized` or `Declined`.

## Trade-offs

- Invalid requests return `201 Created` with `Status = Rejected` to match the exercise wording exactly, even though many production payment APIs would instead use a client error response.
- The solution uses an in-memory repository because the exercise explicitly allows a test-double style storage approach.
- The automated tests focus on request validation, domain rules, and API behavior rather than trying to unit test every simple DTO or thin wrapper.
