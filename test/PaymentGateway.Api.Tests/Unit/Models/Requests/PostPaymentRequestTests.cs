using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.Api.Tests.Unit.Models.Requests;

public class PostPaymentRequestTests
{
    [Fact]
    public void Validate_whenRequestIsValid_thenReturnsValidResult()
    {
        // Arrange
        var request = CreateValidRequest();

        // Act
        var result = Validate(request);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Validate_whenCardNumberIsMissing_thenReturnsInvalidResult()
    {
        // Arrange
        var request = CreateValidRequest();
        request.CardNumber = string.Empty;

        // Act
        var result = Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.MemberNames.Contains(nameof(PostPaymentRequest.CardNumber)));
    }

    [Fact]
    public void Validate_whenCardNumberIsTooShort_thenReturnsInvalidResult()
    {
        // Arrange
        var request = CreateValidRequest();
        request.CardNumber = "1234567890123";

        // Act
        var result = Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.MemberNames.Contains(nameof(PostPaymentRequest.CardNumber)));
    }

    [Fact]
    public void Validate_whenExpiryMonthIsOutsideValidRange_thenReturnsInvalidResult()
    {
        // Arrange
        var request = CreateValidRequest();
        request.ExpiryMonth = 13;

        // Act
        var result = Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.MemberNames.Contains(nameof(PostPaymentRequest.ExpiryMonth)));
    }

    [Fact]
    public void Validate_whenExpiryYearIsBeforeCurrentYear_thenReturnsInvalidResult()
    {
        // Arrange
        var request = CreateValidRequest();
        request.ExpiryYear = DateTime.UtcNow.Year - 1;

        // Act
        var result = Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.MemberNames.Contains(nameof(PostPaymentRequest.ExpiryYear)));
    }

    [Fact]
    public void Validate_whenExpiryYearIsTooFarInFuture_thenReturnsInvalidResult()
    {
        // Arrange
        var request = CreateValidRequest();
        request.ExpiryYear = DateTime.UtcNow.Year + 21;

        // Act
        var result = Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.MemberNames.Contains(nameof(PostPaymentRequest.ExpiryYear)));
    }

    [Fact]
    public void Validate_whenCurrencyLengthIsInvalid_thenReturnsInvalidResult()
    {
        // Arrange
        var request = CreateValidRequest();
        request.Currency = "GB";

        // Act
        var result = Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.MemberNames.Contains(nameof(PostPaymentRequest.Currency)));
    }

    [Fact]
    public void Validate_whenAmountIsNotPositive_thenReturnsInvalidResult()
    {
        // Arrange
        var request = CreateValidRequest();
        request.Amount = 0;

        // Act
        var result = Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.MemberNames.Contains(nameof(PostPaymentRequest.Amount)));
    }

    [Fact]
    public void Validate_whenCvvLengthIsInvalid_thenReturnsInvalidResult()
    {
        // Arrange
        var request = CreateValidRequest();
        request.Cvv = "12";

        // Act
        var result = Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.MemberNames.Contains(nameof(PostPaymentRequest.Cvv)));
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

    private static ValidationTestResult Validate(PostPaymentRequest request)
    {
        var errors = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(
            request,
            new ValidationContext(request),
            errors,
            validateAllProperties: true);

        return new ValidationTestResult(isValid, errors);
    }

    private sealed record ValidationTestResult(
        bool IsValid,
        IReadOnlyCollection<ValidationResult> Errors);
}
