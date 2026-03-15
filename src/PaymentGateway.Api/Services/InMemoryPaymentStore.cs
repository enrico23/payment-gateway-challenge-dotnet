namespace PaymentGateway.Api.Services;

public sealed class InMemoryPaymentStore
{
    public List<Payment> Payments { get; } = [];
}
