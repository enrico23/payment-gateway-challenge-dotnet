using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services;

public sealed class InMemoryPaymentStore
{
    public List<PostPaymentResponse> Payments { get; } = [];
}

public interface IPaymentsRepository
{
    void Add(PostPaymentResponse payment);
    PostPaymentResponse? Get(Guid id);
}

public sealed class PaymentsRepository(InMemoryPaymentStore store) : IPaymentsRepository
{
    public void Add(PostPaymentResponse payment)
    {
        store.Payments.Add(payment);
    }

    public PostPaymentResponse? Get(Guid id)
    {
        return store.Payments.FirstOrDefault(p => p.Id == id);
    }
}