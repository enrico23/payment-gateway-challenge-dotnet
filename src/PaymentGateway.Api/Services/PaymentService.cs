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

        var payment = new PaymentResponse
        {
            Id = Guid.NewGuid(),
            Status = MapStatus(bankResult),
            CardNumberLastFour = request.CardNumber[^4..],
            ExpiryMonth = request.ExpiryMonth,
            ExpiryYear = request.ExpiryYear,
            Currency = request.Currency,
        };

        paymentsRepository.Add(payment);

        return payment;
    }

    private PaymentStatus MapStatus(BankPaymentResult bankResult)
    {
        if (bankResult.IsUnavailable)
            return PaymentStatus.Rejected;

        if (bankResult.IsSuccess && bankResult.Authorized == true)
            return PaymentStatus.Authorized;

        if (bankResult.IsSuccess && bankResult.Authorized == false)
        {
            return PaymentStatus.Declined;
        }

        return PaymentStatus.Rejected;
    }
}
