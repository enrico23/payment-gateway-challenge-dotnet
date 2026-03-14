namespace PaymentGateway.Api.Tests.Integration;

public class GetPaymentByIdTests : PaymentsTestBase
{
    [Fact]
    public async Task RetrievesAPaymentSuccessfully()
    {
        // Arrange
        var store = new InMemoryPaymentStore();
        var randomCardNumber = _random.Next(1111, 9999);

        var payment = new PaymentResponse
        {
            Id = Guid.NewGuid(),
            ExpiryYear = _random.Next(2023, 2030),
            ExpiryMonth = _random.Next(1, 12),
            Amount = _random.Next(1, 10000),
            CardNumberLastFour = randomCardNumber.ToString(),
            Currency = "GBP"
        };
        store.Payments.Add(payment);

        var client = CreateClient(store);

        // Act
        var response = await client.GetAsync($"/api/Payments/{payment.Id}");
        var paymentResponse = await response.Content.ReadFromJsonAsync<PaymentResponse>();
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(paymentResponse);
    }

    [Fact]
    public async Task Returns404IfPaymentNotFound()
    {
        // Arrange
        var store = new InMemoryPaymentStore();
        var client = CreateClient(store);

        // Act
        var response = await client.GetAsync($"/api/Payments/{Guid.NewGuid()}");
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}