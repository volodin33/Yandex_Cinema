using System.Reflection;
using System.Text.Json;
using events.Kafka;
using events.Kafka.Consumers;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddMediatR(cfg => { cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()); });

builder.Services.Configure<KafkaOptions>(builder.Configuration.GetSection("Kafka"));

builder.Services.AddSingleton<KafkaProducer>();
builder.Services.AddHostedService<MovieEventsConsumer>();
builder.Services.AddHostedService<UserEventsConsumer>();
builder.Services.AddHostedService<PaymentEventsConsumer>();
builder.Services.AddHealthChecks();

var app = builder.Build();
app.MapHealthChecks("/health", new HealthCheckOptions { ResponseWriter = WriteJson });
app.MapHealthChecks("/api/events/health", new HealthCheckOptions { ResponseWriter = WriteJson });
app.MapOpenApi();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "My API v1");
    options.RoutePrefix = "swagger";
});

app.MapControllers();
app.Run();

static Task WriteJson(HttpContext context, HealthReport report)
{
    context.Response.ContentType = "application/json";
    return context.Response.WriteAsync(JsonSerializer.Serialize(new
    {
        status = report.Status == HealthStatus.Healthy
    }));
}