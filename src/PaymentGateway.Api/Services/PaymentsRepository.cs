using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services;

public interface IPaymentsRepository
{
    void Add(PaymentResponse payment);
    PaymentResponse? Get(Guid id);
}

public sealed class PaymentsRepository(InMemoryPaymentStore store) : IPaymentsRepository
{
    public void Add(PaymentResponse payment)
    {
        store.Payments.Add(payment);
    }

    public PaymentResponse? Get(Guid id)
    {
        return store.Payments.FirstOrDefault(p => p.Id == id);
    }
}