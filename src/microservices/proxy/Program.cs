using proxy;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<ProxyOptions>(builder.Configuration.GetSection("Proxy"));

builder.Services.AddScoped<RouteSelector>();
builder.Services.AddScoped<Proxy>();
builder.Services.AddHttpClient("proxy", c => c.Timeout = TimeSpan.FromSeconds(15));
builder.Services.AddHealthChecks();

var app = builder.Build();
app.MapHealthChecks("/health");
app.Map("/{**path}", async (HttpContext ctx, Proxy proxy) => await proxy.HandleAsync(ctx));
app.Run();