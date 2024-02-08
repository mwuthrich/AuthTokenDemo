using System.Net;
using System.Security.Cryptography;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Retry;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();
builder.Services.AddTransient<IAuthTokenProvider, AuthTokenProvider>();
builder.Services.AddSingleton<AuthenticatedPipelineBuilder>();
builder.Services.AddHttpClient<ResilientClientWrapper>(client =>
{
    client.BaseAddress = new Uri("https://localhost:5001");
})
.AddHttpMessageHandler<AuthenticatedMessageHandler>();

builder.Services.AddHttpClient<Things2Controller>(client =>
{
    client.BaseAddress = new Uri("https://localhost:5001");
})
.AddHttpMessageHandler<AuthenticatedMessageHandler>()
.AddResilienceHandler("refresh_auth_token", (builder, context) => builder.AddRetry(new RetryStrategyOptions<HttpResponseMessage>
{
    ShouldHandle = args => args.Outcome switch
    {
        { Exception: HttpRequestException { StatusCode: HttpStatusCode.Unauthorized } } => PredicateResult.True(),
        _ => PredicateResult.False()
    },
    MaxRetryAttempts = 1,
    OnRetry = args =>
    {
        var cache = context.ServiceProvider.GetRequiredService<IMemoryCache>();
        cache.Remove(Config.CacheKey);
        return default;
    }
}));


builder.Services.AddControllers();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();

app.Run();
