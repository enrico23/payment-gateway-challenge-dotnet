using PaymentGateway.Api.Models;

namespace PaymentGateway.Api.Tests.Integration;

public class ProcessPaymentTests : PaymentsTestBase
{
    [Theory]
    [InlineData("4242424242424242", 1, 2028, "GBP", 100, "123", "4242")]
    public async Task ProcessesPaymentSuccessfully_WhenBankAuthorizes(
        string cardNumber,
        int expiryMonth,
        int expiryYear,
        string currency,
        int amount,
        string cvv,
        string expectedLastFourDigits)
    {
        AcquiringBankClient
            .ProcessPaymentAsync(Arg.Any<PostPaymentRequest>(), Arg.Any<CancellationToken>())
            .Returns(BankPaymentResult.Success(authorized: true, authorizationCode: "auth-code"));

        var request = new PostPaymentRequest
        {
            CardNumber = cardNumber,
            ExpiryMonth = expiryMonth,
            ExpiryYear = expiryYear,
            Currency = currency,
            Amount = amount,
            Cvv = cvv
        };

        var response = await Client.PostAsJsonAsync("/api/Payments", request);
        var paymentResponse = await response.Content.ReadFromJsonAsync<PaymentResponse>(JsonOptions);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(paymentResponse);
        Assert.NotEqual(Guid.Empty, paymentResponse!.Id);
        Assert.Equal(PaymentStatus.Authorized, paymentResponse.Status);
        Assert.Equal(expectedLastFourDigits, paymentResponse.CardNumberLastFour);
        Assert.Equal(request.Amount, paymentResponse.Amount);
        Assert.Single(DataStore.Payments);
    }

    [Fact]
    public async Task ProcessesPaymentSuccessfully_WhenBankDeclines()
    {
        AcquiringBankClient
            .ProcessPaymentAsync(Arg.Any<PostPaymentRequest>(), Arg.Any<CancellationToken>())
            .Returns(BankPaymentResult.Success(authorized: false, authorizationCode: string.Empty));

        var request = new PostPaymentRequest
        {
            CardNumber = "4242424242424242",
            ExpiryMonth = 12,
            ExpiryYear = 2028,
            Currency = "GBP",
            Amount = 100,
            Cvv = "123"
        };

        var response = await Client.PostAsJsonAsync("/api/Payments", request);
        var paymentResponse = await response.Content.ReadFromJsonAsync<PaymentResponse>(JsonOptions);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(paymentResponse);
        Assert.Equal(PaymentStatus.Declined, paymentResponse!.Status);
        Assert.Single(DataStore.Payments);
    }

    [Fact]
    public async Task ReturnsBadGateway_WhenAcquiringBankIsUnavailable()
    {
        AcquiringBankClient
            .ProcessPaymentAsync(Arg.Any<PostPaymentRequest>(), Arg.Any<CancellationToken>())
            .Returns(BankPaymentResult.Unavailable());

        var request = new PostPaymentRequest
        {
            CardNumber = "4242424242424240",
            ExpiryMonth = 12,
            ExpiryYear = 2028,
            Currency = "GBP",
            Amount = 100,
            Cvv = "123"
        };

        var response = await Client.PostAsJsonAsync("/api/Payments", request);

        Assert.Equal(HttpStatusCode.BadGateway, response.StatusCode);
        Assert.Empty(DataStore.Payments);
    }

    [Theory]
    [InlineData("1234", 12, 2028, "GBP", 100, "123")]
    [InlineData("4242424242424242", 0, 2028, "GBP", 100, "123")]
    [InlineData("4242424242424242", 13, 2028, "GBP", 100, "123")]
    [InlineData("4242424242424242", 12, 2028, "GB", 100, "123")]
    [InlineData("4242424242424242", 12, 2028, "GBP", 0, "123")]
    [InlineData("4242424242424242", 12, 2028, "GBP", 100, "12")]
    public async Task ReturnsBadRequestWhenRequestIsInvalid(
        string cardNumber,
        int expiryMonth,
        int expiryYear,
        string currency,
        int amount,
        string cvv)
    {
        var request = new PostPaymentRequest
        {
            CardNumber = cardNumber,
            ExpiryMonth = expiryMonth,
            ExpiryYear = expiryYear,
            Currency = currency,
            Amount = amount,
            Cvv = cvv
        };

        var response = await Client.PostAsJsonAsync("/api/Payments", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        await AcquiringBankClient
            .DidNotReceive()
            .ProcessPaymentAsync(Arg.Any<PostPaymentRequest>(), Arg.Any<CancellationToken>());
        Assert.Empty(DataStore.Payments);
    }
}
