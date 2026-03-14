namespace PaymentGateway.Api.Clients;

public sealed class BankPaymentResult
{
    public bool IsSuccess { get; init; }
    public bool IsUnavailable { get; init; }
    public bool? Authorized { get; init; }
    public string AuthorizationCode { get; init; } = string.Empty;
    public string ErrorMessage { get; init; } = string.Empty;

    public static BankPaymentResult Success(bool authorized, string authorizationCode) =>
        new()
        {
            IsSuccess = true,
            Authorized = authorized,
            AuthorizationCode = authorizationCode
        };

    public static BankPaymentResult Unavailable() =>
        new()
        {
            IsUnavailable = true,
            ErrorMessage = "Acquiring bank is unavailable."
        };

    public static BankPaymentResult Failed(string errorMessage) =>
        new()
        {
            ErrorMessage = errorMessage
        };
}