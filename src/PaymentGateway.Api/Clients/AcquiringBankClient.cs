using System.Net;

namespace PaymentGateway.Api.Clients;

public interface IAcquiringBankClient
{
    Task<BankPaymentResult> ProcessPaymentAsync(
          PostPaymentRequest request,
          CancellationToken cancellationToken = default);
}

public sealed class AcquiringBankClient(HttpClient httpClient)
    : IAcquiringBankClient
{
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
