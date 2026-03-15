namespace PaymentGateway.Api.Tests.Integration.PaymentsControllerTests;

public class GetPaymentByIdTests : PaymentsTestBase
{
    [Fact]
    public async Task RetrievesAPaymentSuccessfully()
    {
        // Arrange
        var nextMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddMonths(1);
        var payment = Payment.Create(
            new PostPaymentRequest
            {
                CardNumber = "4242424242424242",
                ExpiryYear = nextMonth.Year,
                ExpiryMonth = nextMonth.Month,
                Amount = _random.Next(1, 10000),
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
    }

    [Fact]
    public async Task Returns404IfPaymentNotFound()
    {
        // Act
        var response = await Client.GetAsync($"/api/Payments/{Guid.NewGuid()}");
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
