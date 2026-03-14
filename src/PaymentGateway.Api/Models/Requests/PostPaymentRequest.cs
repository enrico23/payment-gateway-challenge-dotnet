using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.Api.Models.Requests;

public sealed class PostPaymentRequest
{
    [Required]
    [RegularExpression(@"^\d{12,19}$")]
    public string CardNumber { get; set; } = string.Empty;

    [Range(1, 12)]
    public int ExpiryMonth { get; set; }

    [Range(2000, 2100)]
    public int ExpiryYear { get; set; }

    [Required]
    [StringLength(3, MinimumLength = 3)]
    public string Currency { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int Amount { get; set; }

    [Required]
    [RegularExpression(@"^\d{3,4}$")]
    public string Cvv { get; set; } = string.Empty;
}