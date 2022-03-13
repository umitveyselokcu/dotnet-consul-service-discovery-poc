namespace WeatherService.Config;

public class ConsulPayload<T>
{
    public ConsulPayload(T settings)
    {
        this.WeatherForecastSettings = settings;
    }

    public T WeatherForecastSettings { get; set;}
}