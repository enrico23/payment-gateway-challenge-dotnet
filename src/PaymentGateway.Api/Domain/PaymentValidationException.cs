using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.Api.Domain;

/// <summary>
/// 
/// </summary>
/// <param name="errors"></param>
public sealed class PaymentValidationException(IReadOnlyCollection<ValidationResult> errors)
    : Exception("Payment validation failed.")
{
    /// <summary>
    /// 
    /// </summary>
    public IReadOnlyCollection<ValidationResult> Errors { get; } = errors;
}
