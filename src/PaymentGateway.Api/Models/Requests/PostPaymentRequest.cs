using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.Api.Models.Requests;

/// <summary>
/// Payment request sent by a merchant.
/// </summary>
public sealed class PostPaymentRequest : IValidatableObject
{
    /// <summary>
    /// Card number as 14 to 19 digits.
    /// </summary>
    [Required]
    [RegularExpression(@"^\d{14,19}$")]
    public string CardNumber { get; set; } = string.Empty;

    /// <summary>
    /// Card expiry month from 1 to 12.
    /// </summary>
    [Range(1, 12)]
    public int ExpiryMonth { get; set; }

    /// <summary>
    /// Card expiry year.
    /// </summary>
    [Range(2026, 2050)]
    public int ExpiryYear { get; set; }

    /// <summary>
    /// Three-letter ISO currency code.
    /// </summary>
    [Required]
    [StringLength(3, MinimumLength = 3)]
    public string Currency { get; set; } = string.Empty;

    /// <summary>
    /// Amount in minor currency units.
    /// </summary>
    [Range(1, int.MaxValue)]
    public int Amount { get; set; }

    /// <summary>
    /// Card verification value as 3 or 4 digits.
    /// </summary>
    [Required]
    [RegularExpression(@"^\d{3,4}$")]
    public string Cvv { get; set; } = string.Empty;

    /// <summary>
    /// Applies dynamic sanity checks that cannot be expressed with static attributes.
    /// </summary>
    /// <param name="validationContext">Validation context.</param>
    /// <returns>Validation errors, if any.</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var currentYear = DateTime.UtcNow.Year;
        var maximumYear = currentYear + 20;

        if (ExpiryYear < currentYear || ExpiryYear > maximumYear)
        {
            yield return new ValidationResult(
                $"Expiry year must be between {currentYear} and {maximumYear}.",
                [nameof(ExpiryYear)]);
        }
    }
}
