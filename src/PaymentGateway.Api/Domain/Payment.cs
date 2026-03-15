using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.Api.Domain;

/// <summary>
/// Represents a processed payment.
/// </summary>
public sealed class Payment
{
    private static readonly HashSet<string> SupportedCurrencies = new(StringComparer.Ordinal)
    {
        "GBP",
        "USD",
        "EUR"
    };

    private Payment(
        Guid id,
        PaymentStatus status,
        string cardNumberLastFour,
        int expiryMonth,
        int expiryYear,
        string currency,
        int amount)
    {
        Id = id;
        Status = status;
        CardNumberLastFour = cardNumberLastFour;
        ExpiryMonth = expiryMonth;
        ExpiryYear = expiryYear;
        Currency = currency;
        Amount = amount;
    }

    /// <summary>
    /// Payment identifier.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Payment status.
    /// </summary>
    public PaymentStatus Status { get; }

    /// <summary>
    /// Last four digits of the card number.
    /// </summary>
    public string CardNumberLastFour { get; }

    /// <summary>
    /// Card expiry month.
    /// </summary>
    public int ExpiryMonth { get; }

    /// <summary>
    /// Card expiry year.
    /// </summary>
    public int ExpiryYear { get; }

    /// <summary>
    /// Payment currency.
    /// </summary>
    public string Currency { get; }

    /// <summary>
    /// Payment amount in minor currency units.
    /// </summary>
    public int Amount { get; }

    /// <summary>
    /// Validates whether a payment can be created from the request.
    /// </summary>
    /// <param name="request">Payment details.</param>
    public static void EnsureCanCreate(PostPaymentRequest request)
    {
        var errors = ValidateBusinessRules(request);

        if (errors.Count > 0)
            throw new PaymentValidationException(errors);
    }

    /// <summary>
    /// Creates a payment from the request.
    /// </summary>
    /// <param name="request">Payment details.</param>
    /// <param name="status">Payment status.</param>
    /// <returns>The created payment.</returns>
    public static Payment Create(PostPaymentRequest request, PaymentStatus status)
    {
        var errors = ValidateBusinessRules(request);

        if (errors.Count > 0)
            throw new PaymentValidationException(errors);

        return new Payment(
            Guid.NewGuid(),
            status,
            request.CardNumber[^4..],
            request.ExpiryMonth,
            request.ExpiryYear,
            request.Currency,
            request.Amount);
    }

    /// <summary>
    /// Converts the payment to the API response model.
    /// </summary>
    /// <returns>The payment response.</returns>
    public PaymentResponse ToResponse()
    {
        return new PaymentResponse
        {
            Id = Id,
            Status = Status,
            CardNumberLastFour = CardNumberLastFour,
            ExpiryMonth = ExpiryMonth,
            ExpiryYear = ExpiryYear,
            Currency = Currency,
            Amount = Amount
        };
    }

    private static List<ValidationResult> ValidateBusinessRules(PostPaymentRequest request)
    {
        var errors = new List<ValidationResult>();

        if (!string.IsNullOrWhiteSpace(request.Currency) && !SupportedCurrencies.Contains(request.Currency))
        {
            errors.Add(new ValidationResult(
                "Currency must be one of the supported ISO currency codes.",
                [nameof(PostPaymentRequest.Currency)]));
        }

        if (request.ExpiryYear > 0 && request.ExpiryMonth is >= 1 and <= 12)
        {
            var now = DateTime.UtcNow;
            var currentMonth = new DateOnly(now.Year, now.Month, 1);
            var expiryMonth = new DateOnly(request.ExpiryYear, request.ExpiryMonth, 1);

            if (expiryMonth < currentMonth)
            {
                errors.Add(new ValidationResult(
                    "Expiry month and year must not be before the current month.",
                    [nameof(PostPaymentRequest.ExpiryMonth), nameof(PostPaymentRequest.ExpiryYear)]));
            }
        }

        return errors;
    }
}
