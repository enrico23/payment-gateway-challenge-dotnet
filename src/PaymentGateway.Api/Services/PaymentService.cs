namespace PaymentGateway.Api.Services;

public interface IPaymentService
{
    Task<PaymentResponse> ProcessAsync(PostPaymentRequest request, CancellationToken cancellationToken = default);
}

public class PaymentService(
    IPaymentsRepository paymentsRepository,
    IAcquiringBankClient bankClient) 
    : IPaymentService
{
    public async Task<PaymentResponse> ProcessAsync(PostPaymentRequest request, CancellationToken cancellationToken = default)
    {
        var bankResult = await bankClient.ProcessPaymentAsync(request, cancellationToken);

        if (bankResult.IsUnavailable)
            throw new InvalidOperationException("The acquiring bank is unavailable.");
       
        if (!bankResult.IsSuccess || bankResult.Authorized is null)
            throw new InvalidOperationException("The payment could not be processed.");
        
        var payment = new PaymentResponse
        {
            Id = Guid.NewGuid(),
            Amount = request.Amount,
            Status = bankResult.Authorized.Value
                  ? PaymentStatus.Authorized
                  : PaymentStatus.Declined,
            CardNumberLastFour = request.CardNumber[^4..],
            ExpiryMonth = request.ExpiryMonth,
            ExpiryYear = request.ExpiryYear,
            Currency = request.Currency,
        };

        paymentsRepository.Add(payment);

        return payment;
    }
}
