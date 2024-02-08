using System.Net.Http.Headers;
using Microsoft.Extensions.Caching.Memory;

public class AuthenticatedMessageHandler : DelegatingHandler
{
    private readonly IAuthTokenProvider _authTokenProvider;
    private readonly IMemoryCache _memoryCache;

    public AuthenticatedMessageHandler(IAuthTokenProvider authTokenProvider, IMemoryCache memoryCache)
    {
        _authTokenProvider = authTokenProvider;
        _memoryCache = memoryCache;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var authToken = await _memoryCache.GetOrCreateAsync(Config.CacheKey, async entry =>
        {
            //entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            return await _authTokenProvider.GetToken();
        });

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        return await base.SendAsync(request, cancellationToken);
    }
}