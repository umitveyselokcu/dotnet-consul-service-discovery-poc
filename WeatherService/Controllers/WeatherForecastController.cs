using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WeatherService.Config;
using WeatherService.Dto;
using WeatherService.Services;

namespace WeatherService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IOptionsSnapshot<WeatherForecastSettings> _consulConfigs;
        private readonly IHumidityProxyService _humidityProxyService;
        public WeatherForecastController(ILogger<WeatherForecastController> logger, IOptionsSnapshot<WeatherForecastSettings> consulConfigs, IHumidityProxyService humidityProxyService)
        {
            _logger = logger;
            _consulConfigs = consulConfigs;
            _humidityProxyService = humidityProxyService;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<WeatherForecast> Get()
        {
            _logger.LogInformation($"Consul config: ForecastType: {_consulConfigs.Value.CityName} , AllCities: {_consulConfigs.Value.AllCities}");

            var humidity = await _humidityProxyService.GetAsync("Humidity");
            
            return new WeatherForecast
            {
                Date = DateTime.Now,
                TemperatureC = Random.Shared.Next(-20, 55),
                City = _consulConfigs.Value.CityName,
                ConsulConfig = $"ConsulConfig: ForecastType: {_consulConfigs.Value.CityName} , AllCities: {_consulConfigs.Value.AllCities}",
                Humidity = humidity.CurrentHumidity
            };
        }
    }
}