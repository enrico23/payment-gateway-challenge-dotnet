using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Clients;

/// <summary>
/// Payment response returned by the acquiring bank.
/// </summary>
public sealed class BankPaymentResponse
{
    [JsonPropertyName("authorized")]
    /// <summary>
    /// Indicates whether the payment was authorized.
    /// </summary>
    public bool Authorized { get; set; }

    [JsonPropertyName("authorization_code")]
    /// <summary>
    /// Authorization code returned by the bank.
    /// </summary>
    public string AuthorizationCode { get; set; } = string.Empty;
}
