
using System.Net;
using System.Text.Json;

public class TestMessageHandler : HttpMessageHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.Headers.Authorization?.Parameter == "password!")
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(new Thing { Name = "Bob" }))
            };
        }

        return new HttpResponseMessage(HttpStatusCode.Unauthorized);
    }
}
