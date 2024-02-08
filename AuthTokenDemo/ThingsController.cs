using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class ThingsController
{
    private readonly ResilientClientWrapper _httpClient;

    public ThingsController(ResilientClientWrapper httpClient)
    {
        _httpClient = httpClient;
    }

    [HttpGet("{id}")]
    public async Task<Thing?> GetThing(int id)
    {
        var response = await _httpClient.SendAsync(client => client.GetAsync($"/api/things/{id}"));
        if (response == null) return null;
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<Thing>(content);
    }

    [HttpPost]
    public async Task<Thing?> CreateThing(Thing thing)
    {
        var response = await _httpClient.SendAsync(client => client.PostAsJsonAsync("/api/things", thing));
        if (response == null) return null;
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<Thing>(content);
    }
}