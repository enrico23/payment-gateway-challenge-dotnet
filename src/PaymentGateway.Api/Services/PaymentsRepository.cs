namespace PaymentGateway.Api.Services;

/// <summary>
/// 
/// </summary>
public interface IPaymentsRepository
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="payment"></param>
    void Add(Payment payment);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Payment? Get(Guid id);
}

/// <summary>
/// 
/// </summary>
/// <param name="store"></param>
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
