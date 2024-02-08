using System.Net;
using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.Retry;

public class AuthenticatedPipelineBuilder
{
    private readonly ResiliencePipeline _pipeline;
    private readonly IMemoryCache _memoryCache;

    public AuthenticatedPipelineBuilder(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
        _pipeline = new ResiliencePipelineBuilder()
            .AddRetry(
                new RetryStrategyOptions
                {
                    ShouldHandle = new PredicateBuilder().Handle<HttpRequestException>(
                        ex => ex.StatusCode == HttpStatusCode.Unauthorized
                    ),
                    MaxRetryAttempts = 1,
                    OnRetry = args =>
                    {
                        _memoryCache.Remove(Config.CacheKey);
                        return default;
                    }
                }
            )
            .Build();
    }

    public ResiliencePipeline Pipeline => _pipeline;
}
