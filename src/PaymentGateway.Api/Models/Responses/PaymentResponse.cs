namespace PaymentGateway.Api.Models.Responses;

/// <summary>
/// Payment details returned by the API.
/// </summary>
public sealed class PaymentResponse
{
    /// <summary>
    /// Payment identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Payment status.
    /// </summary>
    public PaymentStatus Status { get; set; }

    /// <summary>
    /// Last four digits of the card number.
    /// </summary>
    public string CardNumberLastFour { get; set; } = string.Empty;

    /// <summary>
    /// Card expiry month.
    /// </summary>
    public int ExpiryMonth { get; set; }

    /// <summary>
    /// Card expiry year.
    /// </summary>
    public int ExpiryYear { get; set; }

    /// <summary>
    /// Payment currency.
    /// </summary>
    public string Currency { get; set; } = string.Empty;

    /// <summary>
    /// Payment amount in minor currency units.
    /// </summary>
    public int Amount { get; set; }
}
