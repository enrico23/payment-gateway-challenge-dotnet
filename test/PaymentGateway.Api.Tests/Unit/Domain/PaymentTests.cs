namespace PaymentGateway.Api.Tests.Unit.Domain;

public class PaymentTests
{
    [Fact]
    public void Create_WhenRequestIsValid_ThenCreatesPaymentWithMaskedCardNumber()
    {
        // Arrange
        var request = CreateValidRequest();

        // Act
        var payment = Payment.Create(request, PaymentStatus.Authorized);

        // Assert
        Assert.Equal(PaymentStatus.Authorized, payment.Status);
        Assert.Equal("4242", payment.CardNumberLastFour);
        Assert.Equal(request.ExpiryMonth, payment.ExpiryMonth);
        Assert.Equal(request.ExpiryYear, payment.ExpiryYear);
        Assert.Equal(request.Currency, payment.Currency);
        Assert.Equal(request.Amount, payment.Amount);
    }

    [Fact]
    public void Create_WhenExpiryMonthIsCurrentMonth_ThenCreatesPayment()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var request = CreateValidRequest();
        request.ExpiryMonth = now.Month;
        request.ExpiryYear = now.Year;

        // Act
        var payment = Payment.Create(request, PaymentStatus.Authorized);

        // Assert
        Assert.Equal(now.Month, payment.ExpiryMonth);
        Assert.Equal(now.Year, payment.ExpiryYear);
    }

    [Fact]
    public void Create_WhenExpiryMonthHasPassed_ThenThrowsPaymentValidationException()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var expiredDate = new DateTime(now.Year, now.Month, 1).AddMonths(-1);
        var request = CreateValidRequest();
        request.ExpiryMonth = expiredDate.Month;
        request.ExpiryYear = expiredDate.Year;

        // Act
        var action = () => Payment.Create(request, PaymentStatus.Authorized);

        // Assert
        var exception = Assert.Throws<PaymentValidationException>(action);
        Assert.Contains(exception.Errors, e => e.MemberNames.Contains(nameof(PostPaymentRequest.ExpiryMonth)));
        Assert.Contains(exception.Errors, e => e.MemberNames.Contains(nameof(PostPaymentRequest.ExpiryYear)));
    }

    [Fact]
    public void Create_WhenCurrencyIsUnsupported_ThenThrowsPaymentValidationException()
    {
        // Arrange
        var request = CreateValidRequest();
        request.Currency = "AUD";

        // Act
        var action = () => Payment.Create(request, PaymentStatus.Authorized);

        // Assert
        var exception = Assert.Throws<PaymentValidationException>(action);
        Assert.Contains(exception.Errors, e => e.MemberNames.Contains(nameof(PostPaymentRequest.Currency)));
    }

    private static PostPaymentRequest CreateValidRequest()
    {
        var nextMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddMonths(1);

        return new PostPaymentRequest
        {
            CardNumber = "4242424242424242",
            ExpiryMonth = nextMonth.Month,
            ExpiryYear = nextMonth.Year,
            Currency = "GBP",
            Amount = 100,
            Cvv = "123"
        };
    }
}
