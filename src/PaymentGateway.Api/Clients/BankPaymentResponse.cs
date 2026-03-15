using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Clients;

/// <summary>
/// Payment response returned by the acquiring bank.
/// </summary>
public sealed class BankPaymentResponse
{
    [JsonPropertyName("authorized")]
    public bool Authorized { get; set; }

    [JsonPropertyName("authorization_code")]
    public string AuthorizationCode { get; set; } = string.Empty;
}
