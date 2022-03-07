namespace WeatherService.Config
{
    public class WeatherForecastSettings
    {
        public string? ForecastType { get; set; }

        public bool AllCities { get; set; } 
    }

    public class WeatherForecastSettingsRequest
    {
        public WeatherForecastSettingsRequest(WeatherForecastSettings weatherForecastSettings)
        {
            WeatherForecastSettings = weatherForecastSettings;
        }

        public WeatherForecastSettings WeatherForecastSettings
        {
            get;
            set;
        }
    }
}
