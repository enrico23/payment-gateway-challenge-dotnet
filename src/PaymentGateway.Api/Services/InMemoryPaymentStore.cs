namespace PaymentGateway.Api.Services;

/// <summary>
/// 
/// </summary>
public sealed class InMemoryPaymentStore
{
    /// <summary>
    /// 
    /// </summary>
    public List<Payment> Payments { get; } = [];
}
