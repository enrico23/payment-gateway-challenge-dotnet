namespace PaymentGateway.Api.Services;

public interface IPaymentService
{
    Task<PaymentResponse> ProcessAsync(PostPaymentRequest request);
}

public class PaymentService(
    IPaymentsRepository paymentsRepository) 
    : IPaymentService
{
    public Task<PaymentResponse> ProcessAsync(PostPaymentRequest request)
    {
        var payment = new PaymentResponse
        {
            Id = Guid.NewGuid(),
            Status = PaymentStatus.Authorized,
            CardNumberLastFour = request.CardNumber,
            ExpiryMonth = request.ExpiryMonth,
            ExpiryYear = request.ExpiryYear,
            Currency = request.Currency,
            Amount = request.Amount
        };

        paymentsRepository.Add(payment);

        return Task.FromResult(payment);
    }
}
