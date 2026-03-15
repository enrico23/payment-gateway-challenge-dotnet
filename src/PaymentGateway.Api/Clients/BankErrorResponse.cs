using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Clients;

/// <summary>
/// Error response returned by the acquiring bank.
/// </summary>
public sealed class BankErrorResponse
{
    [JsonPropertyName("error_message")]
    /// <summary>
    /// Error message from the bank.
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;
}
