using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.Api.Models.Requests;

/// <summary>
/// Represents the request contract used by merchants to submit a card payment for processing.
/// </summary>
public sealed class PostPaymentRequest : IValidatableObject
{
    /// <summary>
    /// The primary account number of the card, expressed as 14 to 19 numeric digits.
    /// </summary>
    [Required]
    [RegularExpression(@"^\d{14,19}$")]
    public string CardNumber { get; set; } = string.Empty;

    /// <summary>
    /// The card expiry month, expressed as a value from 1 to 12.
    /// </summary>
    [Range(1, 12)]
    public int ExpiryMonth { get; set; }

    /// <summary>
    /// The card expiry year. Request validation applies a realistic sanity range, while domain validation enforces actual expiry rules.
    /// </summary>
    [Range(2026, 2050)]
    public int ExpiryYear { get; set; }

    /// <summary>
    /// The three-letter ISO currency code for the payment.
    /// </summary>
    [Required]
    [StringLength(3, MinimumLength = 3)]
    public string Currency { get; set; } = string.Empty;

    /// <summary>
    /// The payment amount in minor currency units, for example 1050 for 10.50 GBP.
    /// </summary>
    [Range(1, int.MaxValue)]
    public int Amount { get; set; }

    /// <summary>
    /// The card verification value, expressed as 3 to 4 numeric digits.
    /// </summary>
    [Required]
    [RegularExpression(@"^\d{3,4}$")]
    public string Cvv { get; set; } = string.Empty;

    /// <summary>
    /// Performs fail-fast sanity validation for values that need dynamic date-based checks at the API boundary.
    /// </summary>
    /// <param name="validationContext">The validation context supplied by the framework.</param>
    /// <returns>A sequence of validation errors when the request contains unrealistic expiry year values.</returns>
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
