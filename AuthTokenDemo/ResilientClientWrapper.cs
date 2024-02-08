using Polly;

public class ResilientClientWrapper
{
    private readonly HttpClient _httpClient;
    private readonly ResiliencePipeline _pipeline;

    public ResilientClientWrapper(HttpClient httpClient,
        AuthenticatedPipelineBuilder pipelineBuilder)
    {
        _httpClient = httpClient;
        _pipeline = pipelineBuilder.Pipeline;
    }

    public async Task<HttpResponseMessage?> SendAsync(Func<HttpClient, Task<HttpResponseMessage>> request)
    {
        return await _pipeline.ExecuteAsync(async _ =>
        {
            var response = await request(_httpClient);
            response.EnsureSuccessStatusCode();
            return response;
        });
    }
}