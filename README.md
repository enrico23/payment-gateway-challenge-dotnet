# Instructions for candidates

This is the .NET version of the Payment Gateway challenge. If you haven't already read this [README.md](https://github.com/cko-recruitment/) on the details of this exercise, please do so now. 

## Template structure
```
src/
    PaymentGateway.Api - a skeleton ASP.NET Core Web API
test/
    PaymentGateway.Api.Tests - an empty xUnit test project
imposters/ - contains the bank simulator configuration. Don't change this

.editorconfig - don't change this. It ensures a consistent set of rules for submissions when reformatting code
docker-compose.yml - configures the bank simulator
PaymentGateway.sln
```

Feel free to change the structure of the solution, use a different test library etc.

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

### Error handling

- Invalid request or failed business validation returns `422 Unprocessable Entity`
- Acquiring bank unavailability returns `502 Bad Gateway`

The use of `422 Unprocessable Entity` for invalid payment requests is an intentional design choice based on common payment-gateway behavior and Checkout.com documentation:

- Checkout.com API usage documentation states that `422` means `Invalid data was sent` and that validation errors are returned as `422 HTTP` responses:
  https://checkoutdocs.readme.io/docs/api-usage
- Checkout.com support documentation states that when invalid data is sent in a payment request, the payment is not processed and a `422` response is returned:
  https://support.checkout.com/hc/en-us/articles/25924251572754-Invalid-data-in-payment-request
- Checkout.com also documents that `422` errors occur before the request reaches the gateway, so the transaction is never created:
  https://support.checkout.com/hc/en-us/articles/28789241477266-Transaction-with-422-error-not-appearing-in-any-of-my-reports

### Testing

The solution includes:

- unit tests for `PostPaymentRequest` input validation
- unit tests for `Payment.Create(...)` business rules
- integration tests for the `PaymentsController`

### Validation approach

The validation split follows Microsoft guidance for separating API model validation from domain invariants:

- ASP.NET Core model validation is used at the request boundary for fail-fast input validation:
  https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation?view=aspnetcore-9.0
- `IValidatableObject` is used for dynamic request-level checks that depend on the current date:
  https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.ivalidatableobject?view=net-9.0
- Domain invariants are enforced in the domain model, following Microsoft guidance on keeping business validation in the domain layer:
  https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/domain-model-layer-validations
