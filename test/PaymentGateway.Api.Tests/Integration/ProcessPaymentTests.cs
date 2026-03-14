namespace PaymentGateway.Api.Tests.Integration;

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
            CardNumber = "4242424242424242",
            ExpiryMonth = 12,
            ExpiryYear = 2028,
            Currency = "GBP",
            Amount = 100,
            Cvv = "123"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/Payments", request);
        var paymentResponse = await response.Content.ReadFromJsonAsync<PaymentResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(paymentResponse);
        Assert.NotEqual(Guid.Empty, paymentResponse!.Id);
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
        // Arrange
        var store = new InMemoryPaymentStore();
        var client = CreateClient(store);

        var request = new PostPaymentRequest
        {
            CardNumber = cardNumber,
            ExpiryMonth = expiryMonth,
            ExpiryYear = expiryYear,
            Currency = currency,
            Amount = amount,
            Cvv = cvv
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/Payments", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
