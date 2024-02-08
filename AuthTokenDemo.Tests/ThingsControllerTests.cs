using Microsoft.Extensions.Caching.Memory;

namespace AuthTokenDemo.Tests;

public class ThingsControllerTests
{
    [Fact]
    public async Task CanRefreshAuthToken_MessageHandler()
    {
        //arrange
        var cache = new MemoryCache(new MemoryCacheOptions());
        var authTokenProvider = Substitute.For<IAuthTokenProvider>();
        authTokenProvider.GetToken().Returns("bad_password", "password!");

        var pipelineBuilder = new AuthenticatedPipelineBuilder(cache);
        var handler = new AuthenticatedMessageHandler(authTokenProvider, cache)
        {
            InnerHandler = new TestMessageHandler(),
        };
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost")
        };

        var client = new ResilientClientWrapper(httpClient, pipelineBuilder);
        var controller = new ThingsController(client);

        //act
        var result = await controller.GetThing(1);

        //assert
        await authTokenProvider.Received(2).GetToken();
        Assert.Equal("Bob", result.Name);
    }
}