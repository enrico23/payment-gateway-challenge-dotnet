namespace PaymentGateway.Api.Tests.Integration.PaymentsControllerTests;

public class GetPaymentByIdTests : PaymentsTestBase
{
    [Fact]
    public async Task GetPayment_WhenPaymentExists_ThenReturnsOk()
    {
        // Arrange
        var nextMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddMonths(1);
        var amount = _random.Next(1, 10000);
        var payment = Payment.Create(
            new PostPaymentRequest
            {
                CardNumber = "4242424242424242",
                ExpiryYear = nextMonth.Year,
                ExpiryMonth = nextMonth.Month,
                Amount = amount,
                Currency = "GBP",
                Cvv = "123"
            },
            PaymentStatus.Authorized);

        DataStore.Payments.Add(payment);

        // Act
        var response = await Client.GetAsync($"/api/Payments/{payment.Id}");
        var paymentResponse = await response.Content.ReadFromJsonAsync<PaymentResponse>(JsonOptions);
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(paymentResponse);
        Assert.Equal(payment.Id, paymentResponse!.Id);
        Assert.Equal(PaymentStatus.Authorized, paymentResponse.Status);
        Assert.Equal("4242", paymentResponse.CardNumberLastFour);
        Assert.Equal(nextMonth.Month, paymentResponse.ExpiryMonth);
        Assert.Equal(nextMonth.Year, paymentResponse.ExpiryYear);
        Assert.Equal("GBP", paymentResponse.Currency);
        Assert.Equal(amount, paymentResponse.Amount);
    }

    [Fact]
    public async Task GetPayment_WhenPaymentDoesNotExist_ThenReturnsNotFound()
    {
        // Act
        var response = await Client.GetAsync($"/api/Payments/{Guid.NewGuid()}");
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
