using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services;

public sealed class InMemoryPaymentStore
{
    public List<PaymentResponse> Payments { get; } = [];
}
