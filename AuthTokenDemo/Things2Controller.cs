using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class Things2Controller
{
    private readonly HttpClient _httpClient;

    public Things2Controller(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    [HttpGet("{id}")]
    public async Task<Thing?> GetThing(int id)
    {
        var response = await _httpClient.GetAsync($"/api/things/{id}");
        if (response == null) return null;
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<Thing>(content);
    }

    [HttpPost]
    public async Task<Thing?> CreateThing(Thing thing)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/things", thing);
        if (response == null) return null;
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<Thing>(content);
    }
}