using System.Text.Json;
using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Tests.Integration.PaymentsControllerTests;

public abstract class PaymentsTestBase : IDisposable
{
    protected InMemoryPaymentStore DataStore { get; } = new();

    protected IAcquiringBankClient AcquiringBankClient { get; } =
        Substitute.For<IAcquiringBankClient>();

    protected WebApplicationFactory<Program> WebApplicationFactory { get; }

    protected HttpClient Client { get; }

    protected static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    protected readonly Random _random = new();

    protected PaymentsTestBase()
    {
        WebApplicationFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<InMemoryPaymentStore>();
                    services.RemoveAll<IPaymentsRepository>();
                    services.RemoveAll<IPaymentService>();
                    services.RemoveAll<IAcquiringBankClient>();

                    services.AddSingleton(DataStore);
                    services.AddScoped<IPaymentsRepository, PaymentsRepository>();
                    services.AddScoped<IPaymentService, PaymentService>();
                    services.AddSingleton<IAcquiringBankClient>(AcquiringBankClient);
                }));

        Client = WebApplicationFactory.CreateClient();
    }

    public void Dispose()
    {
        Client.Dispose();
        WebApplicationFactory.Dispose();
    }
}
