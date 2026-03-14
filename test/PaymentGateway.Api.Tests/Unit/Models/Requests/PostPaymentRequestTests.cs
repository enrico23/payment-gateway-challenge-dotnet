using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.Api.Tests.Unit.Models.Requests;

public class PostPaymentRequestTests
{
    [Fact]
    public void ValidRequest_PassesValidation()
    {
        var request = new PostPaymentRequest
        {
            CardNumber = "4242424242424242",
            ExpiryMonth = 12,
            ExpiryYear = 2028,
            Currency = "GBP",
            Amount = 100,
            Cvv = "123"
        };

        var result = Validate(request);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void MissingCardNumber_FailsValidation()
    {
        var request = new PostPaymentRequest
        {
            CardNumber = "",
            ExpiryMonth = 12,
            ExpiryYear = 2028,
            Currency = "GBP",
            Amount = 100,
            Cvv = "123"
        };

        var result = Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.MemberNames.Contains(nameof(PostPaymentRequest.CardNumber)));
    }

    [Fact]
    public void InvalidExpiryMonth_FailsValidation()
    {
        var request = new PostPaymentRequest
        {
            CardNumber = "4242424242424242",
            ExpiryMonth = 13,
            ExpiryYear = 2028,
            Currency = "GBP",
            Amount = 100,
            Cvv = "123"
        };

        var result = Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.MemberNames.Contains(nameof(PostPaymentRequest.ExpiryMonth)));
    }

    [Fact]
    public void InvalidCurrency_FailsValidation()
    {
        var request = new PostPaymentRequest
        {
            CardNumber = "4242424242424242",
            ExpiryMonth = 12,
            ExpiryYear = 2028,
            Currency = "GB",
            Amount = 100,
            Cvv = "123"
        };

        var result = Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.MemberNames.Contains(nameof(PostPaymentRequest.Currency)));
    }

    [Fact]
    public void NonPositiveAmount_FailsValidation()
    {
        var request = new PostPaymentRequest
        {
            CardNumber = "4242424242424242",
            ExpiryMonth = 12,
            ExpiryYear = 2028,
            Currency = "GBP",
            Amount = 0,
            Cvv = "123"
        };

        var result = Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.MemberNames.Contains(nameof(PostPaymentRequest.Amount)));
    }

    [Fact]
    public void InvalidCvv_FailsValidation()
    {
        var request = new PostPaymentRequest
        {
            CardNumber = "4242424242424242",
            ExpiryMonth = 12,
            ExpiryYear = 2028,
            Currency = "GBP",
            Amount = 100,
            Cvv = "12"
        };

        var result = Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.MemberNames.Contains(nameof(PostPaymentRequest.Cvv)));
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
