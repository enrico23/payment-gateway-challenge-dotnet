using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Clients;

public sealed class BankErrorResponse
{
    [JsonPropertyName("error_message")]
    public string ErrorMessage { get; set; } = string.Empty;
}
