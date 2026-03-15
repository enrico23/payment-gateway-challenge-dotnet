namespace PaymentGateway.Api.Services;

/// <summary>
/// Processes payment requests.
/// </summary>
public interface IPaymentService
{
    /// <summary>
    /// Processes a payment request and returns the result.
    /// </summary>
    /// <param name="request">The payment request to process.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The processed payment.</returns>
    Task<PaymentResponse> ProcessAsync(PostPaymentRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
/// Handles payment processing.
/// </summary>
public class PaymentService(
    IPaymentsRepository paymentsRepository,
    IAcquiringBankClient bankClient)
    : IPaymentService
{
    /// <inheritdoc />
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
