using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Clients;

/// <summary>
/// Payment request sent to the acquiring bank.
/// </summary>
public sealed class BankPaymentRequest
{
    [JsonPropertyName("card_number")]
    /// <summary>
    /// Card number.
    /// </summary>
    public string CardNumber { get; set; } = string.Empty;

    [JsonPropertyName("expiry_date")]
    /// <summary>
    /// Card expiry date.
    /// </summary>
    public string ExpiryDate { get; set; } = string.Empty;

    [JsonPropertyName("currency")]
    /// <summary>
    /// Payment currency.
    /// </summary>
    public string Currency { get; set; } = string.Empty;

    [JsonPropertyName("amount")]
    /// <summary>
    /// Payment amount in minor currency units.
    /// </summary>
    public int Amount { get; set; }

    [JsonPropertyName("cvv")]
    /// <summary>
    /// Card verification value.
    /// </summary>
    public string Cvv { get; set; } = string.Empty;
}
