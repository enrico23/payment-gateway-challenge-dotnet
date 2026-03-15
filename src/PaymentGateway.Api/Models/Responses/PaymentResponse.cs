namespace PaymentGateway.Api.Models.Responses;

/// <summary>
/// Payment details returned by the API.
/// </summary>
public sealed class PaymentResponse
{
    public Guid Id { get; set; }
    public PaymentStatus Status { get; set; }
    public string CardNumberLastFour { get; set; } = string.Empty;
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public string Currency { get; set; } = string.Empty;
    public int Amount { get; set; }
}
