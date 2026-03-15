using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.Api.Domain;

public sealed class PaymentValidationException(IReadOnlyCollection<ValidationResult> errors)
    : Exception("Payment validation failed.")
{
    public IReadOnlyCollection<ValidationResult> Errors { get; } = errors;
}
