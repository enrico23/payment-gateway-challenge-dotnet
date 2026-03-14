using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Clients;

public sealed class BankPaymentResponse
{
    [JsonPropertyName("authorized")]
    public bool Authorized { get; set; }

    [JsonPropertyName("authorization_code")]
    public string AuthorizationCode { get; set; } = string.Empty;
}
