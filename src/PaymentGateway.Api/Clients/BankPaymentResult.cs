namespace PaymentGateway.Api.Clients;

/// <summary>
/// Result of a call to the acquiring bank.
/// </summary>
public sealed class BankPaymentResult
{
    public bool IsSuccess { get; init; }
    public bool IsUnavailable { get; init; }
    public bool? Authorized { get; init; }
    public string AuthorizationCode { get; init; } = string.Empty;
    public string ErrorMessage { get; init; } = string.Empty;

    /// <summary>
    /// Creates a successful bank result.
    /// </summary>
    /// <param name="authorized">Whether the payment was authorized.</param>
    /// <param name="authorizationCode">Authorization code returned by the bank.</param>
    /// <returns>A successful bank result.</returns>
    public static BankPaymentResult Success(bool authorized, string authorizationCode) =>
        new()
        {
            IsSuccess = true,
            Authorized = authorized,
            AuthorizationCode = authorizationCode
        };

    /// <summary>
    /// Creates a bank-unavailable result.
    /// </summary>
    /// <returns>A bank-unavailable result.</returns>
    public static BankPaymentResult Unavailable() =>
        new()
        {
            IsUnavailable = true,
            ErrorMessage = "Acquiring bank is unavailable."
        };

    /// <summary>
    /// Creates a failed bank result.
    /// </summary>
    /// <param name="errorMessage">Failure message.</param>
    /// <returns>A failed bank result.</returns>
    public static BankPaymentResult Failed(string errorMessage) =>
        new()
        {
            ErrorMessage = errorMessage
        };
}
