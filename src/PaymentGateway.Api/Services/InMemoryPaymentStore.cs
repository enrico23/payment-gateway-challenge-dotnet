namespace PaymentGateway.Api.Services;

/// <summary>
/// In-memory payment store used by the repository.
/// </summary>
public sealed class InMemoryPaymentStore
{
    /// <summary>
    /// Stored payments.
    /// </summary>
    public List<Payment> Payments { get; } = [];
}
