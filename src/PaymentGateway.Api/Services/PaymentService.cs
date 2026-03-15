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
        Payment.EnsureCanCreate(request);

        var bankResult = await bankClient.ProcessPaymentAsync(request, cancellationToken);

        if (bankResult.IsUnavailable)
            throw new InvalidOperationException("The acquiring bank is unavailable.");

        if (!bankResult.IsSuccess || bankResult.Authorized is null)
            throw new InvalidOperationException("The payment could not be processed.");

        var payment = Payment.Create(
            request,
            bankResult.Authorized.Value ? PaymentStatus.Authorized : PaymentStatus.Declined);

        paymentsRepository.Add(payment);

        return payment.ToResponse();
    }
}
