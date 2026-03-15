namespace PaymentGateway.Api.Services;

/// <summary>
/// Stores processed payments.
/// </summary>
public interface IPaymentsRepository
{
    /// <summary>
    /// Adds a payment to the store.
    /// </summary>
    /// <param name="payment">Payment to store.</param>
    void Add(Payment payment);

    /// <summary>
    /// Gets a payment by identifier.
    /// </summary>
    /// <param name="id">Payment identifier.</param>
    /// <returns>The matching payment, if found.</returns>
    Payment? Get(Guid id);
}

/// <summary>
/// Stores payments in memory.
/// </summary>
/// <param name="store">Backing store.</param>
public sealed class PaymentsRepository(InMemoryPaymentStore store) : IPaymentsRepository
{
    /// <inheritdoc />
    public void Add(Payment payment)
    {
        store.Payments.Add(payment);
    }

    /// <inheritdoc />
    public Payment? Get(Guid id)
    {
        return store.Payments.FirstOrDefault(p => p.Id == id);
    }
}
