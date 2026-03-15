namespace PaymentGateway.Api.Clients;

/// <summary>
/// Error response returned by the acquiring bank.
/// </summary>
public sealed class BankErrorResponse
{
    [JsonPropertyName("error_message")]
    public string ErrorMessage { get; set; } = string.Empty;
}
