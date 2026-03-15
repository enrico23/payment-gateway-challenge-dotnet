namespace PaymentGateway.Api.Models;

/// <summary>
/// Payment outcome returned by the gateway.
/// </summary>
public enum PaymentStatus
{
    /// <summary>
    /// Payment was authorized by the acquiring bank.
    /// </summary>
    Authorized,

    /// <summary>
    /// Payment was declined by the acquiring bank.
    /// </summary>
    Declined,

    /// <summary>
    /// Payment was rejected before it was processed.
    /// </summary>
    Rejected
}
