using Consul;
using Newtonsoft.Json;
using WeatherService.Dto;

namespace WeatherService.Services;

public interface IHumidityProxyService
{
    Task<Humidity> GetAsync(string route);
}

public class HumidityProxyService : IHumidityProxyService
{
    private readonly HttpClient _client;
    private readonly ILogger<HumidityProxyService> _logger;

    public HumidityProxyService(HttpClient client, ILogger<HumidityProxyService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<Humidity> GetAsync(string route)
    {
        _logger.LogInformation($"###### HumidityProxyService _client.BaseAddress: {_client.BaseAddress}");
        var response = await _client.GetStringAsync(route);
        return JsonConvert.DeserializeObject<Humidity>(response);
    }
}
