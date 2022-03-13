using HumidityService.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace HumidityService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HumidityController : ControllerBase
    {
        
        private readonly IOptionsSnapshot<HumiditySettings> _consulConfigs;
        public HumidityController(IOptionsSnapshot<HumiditySettings> consulConfigs)
        {
            _consulConfigs = consulConfigs;
        }

        [HttpGet(Name = "GetHumidity")]
        public Humidity Get()
        {
            return !_consulConfigs.Value.DetailedResponse ? 
                new Humidity{ CurrentHumidity = $"{Random.Shared.Next(0, 99)}" } :
                new Humidity{ CurrentHumidity = $"Current humidity is {Random.Shared.Next(0, 99)}%" };
        }
    }
}