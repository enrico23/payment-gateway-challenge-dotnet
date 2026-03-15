namespace PaymentGateway.Api.Services;

public interface IPaymentsRepository
{
    void Add(Payment payment);
    Payment? Get(Guid id);
}

public sealed class PaymentsRepository(InMemoryPaymentStore store) : IPaymentsRepository
{
    public void Add(Payment payment)
    {
        store.Payments.Add(payment);
    }

    public Payment? Get(Guid id)
    {
        return store.Payments.FirstOrDefault(p => p.Id == id);
    }
}
