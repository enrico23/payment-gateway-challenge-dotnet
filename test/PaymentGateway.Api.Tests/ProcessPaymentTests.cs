namespace PaymentGateway.Api.Tests;

public class ProcessPaymentTests : PaymentsTestBase
{
    [Fact]
    public async Task ProcessesPaymentSuccessfully()
    {
        // Arrange
        var store = new InMemoryPaymentStore();
        var client = CreateClient(store);

        var request = new PostPaymentRequest
        {
            CardNumberLastFour = 1234,
            ExpiryMonth = 12,
            ExpiryYear = 2028,
            Currency = "GBP",
            Amount = 100,
            Cvv = 123
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/Payments", request);
        var paymentResponse = await response.Content.ReadFromJsonAsync<PostPaymentResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(paymentResponse);
        Assert.NotEqual(Guid.Empty, paymentResponse!.Id);
    }
}
