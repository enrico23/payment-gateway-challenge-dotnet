namespace PaymentGateway.Api.Tests
{
    public abstract class PaymentsTestBase
    {
        protected readonly Random _random = new();

        protected HttpClient CreateClient(InMemoryPaymentStore store)
        {
            var factory = new WebApplicationFactory<Program>();

            return factory.WithWebHostBuilder(builder =>
                builder.ConfigureServices(services =>
                {
                    services.AddSingleton(store);
                    services.AddScoped<IPaymentsRepository, PaymentsRepository>();
                    services.AddScoped<IPaymentService, PaymentService>();
                }))
                .CreateClient();
        }
    }
}