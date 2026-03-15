using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.Api.Domain;

/// <summary>
/// Raised when a payment request fails domain validation.
/// </summary>
/// <param name="errors">Validation errors.</param>
public sealed class PaymentValidationException(IReadOnlyCollection<ValidationResult> errors)
    : Exception("Payment validation failed.")
{
    /// <summary>
    /// Validation errors returned by the domain.
    /// </summary>
    public IReadOnlyCollection<ValidationResult> Errors { get; } = errors;
}
