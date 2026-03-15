using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var xmlFileName = $"{typeof(Program).Assembly.GetName().Name}.xml";
var xmlFilePath = Path.Combine(AppContext.BaseDirectory, xmlFileName);

// Add services to the container.
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        // Converts enum to string values.
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
        new UnprocessableEntityObjectResult(new ValidationProblemDetails(context.ModelState));
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.IncludeXmlComments(xmlFilePath);
});

builder.Services.AddSingleton<InMemoryPaymentStore>();
builder.Services.AddScoped<IPaymentsRepository, PaymentsRepository>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

// Acquiring Bank Http Client
builder.Services.AddHttpClient<IAcquiringBankClient, AcquiringBankClient>(
    client => client.BaseAddress = new Uri("http://localhost:8080"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// Exposes the entry point type for WebApplicationFactory integration tests.
public partial class Program;
