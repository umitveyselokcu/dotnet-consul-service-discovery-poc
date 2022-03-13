namespace WeatherService.Dto
{
    public class WeatherForecast
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string? City { get; set; }
        
        public string? ConsulConfig { get; set; }
        
        public string? Humidity { get; set; }
    }
}