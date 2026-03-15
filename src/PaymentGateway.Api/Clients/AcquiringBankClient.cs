using System.Net;

namespace PaymentGateway.Api.Clients;

/// <summary>
/// Sends payment requests to the acquiring bank.
/// </summary>
public interface IAcquiringBankClient
{
    /// <summary>
    /// Sends a payment request to the acquiring bank.
    /// </summary>
    /// <param name="request">Payment details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The bank processing result.</returns>
    Task<BankPaymentResult> ProcessPaymentAsync(
          PostPaymentRequest request,
          CancellationToken cancellationToken = default);
}

/// <summary>
/// HTTP client for the acquiring bank.
/// </summary>
public sealed class AcquiringBankClient(HttpClient httpClient)
    : IAcquiringBankClient
{
    /// <inheritdoc />
    public async Task<BankPaymentResult> ProcessPaymentAsync(
         PostPaymentRequest request,
         CancellationToken cancellationToken = default)
    {
        var bankRequest = new BankPaymentRequest
        {
            CardNumber = request.CardNumber,
            ExpiryDate = $"{request.ExpiryMonth:D2}/{request.ExpiryYear}",
            Currency = request.Currency,
            Amount = request.Amount,
            Cvv = request.Cvv
        };

        using var response = await httpClient.PostAsJsonAsync(
            "/payments",
            bankRequest,
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
            return BankPaymentResult.Unavailable();
        
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<BankErrorResponse>(cancellationToken);
            return BankPaymentResult.Failed(error?.ErrorMessage ?? "Bank request failed.");
        }

        var bankResponse = await response.Content.ReadFromJsonAsync<BankPaymentResponse>(cancellationToken);

        return bankResponse is null
            ? BankPaymentResult.Failed("Bank response was empty.")
            : BankPaymentResult.Success(bankResponse.Authorized, bankResponse.AuthorizationCode);
    }
}
