using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WeatherService.Config;

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
        private readonly IOptionsSnapshot<WeatherForecastSettings> _forecasts;
        public WeatherForecastController(ILogger<WeatherForecastController> logger, IOptionsSnapshot<WeatherForecastSettings> forecasts)
        {
            _logger = logger;
            _forecasts = forecasts;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            _logger.LogInformation($"Consul config: ForecastType: {_forecasts.Value.ForecastType} , AllCities: {_forecasts.Value.AllCities}  ");
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)],
                ConsulConfig = $"ConsulConfig: ForecastType: {_forecasts.Value.ForecastType} , AllCities: {_forecasts.Value.AllCities}"
            })
            .ToArray();
        }
    }
}