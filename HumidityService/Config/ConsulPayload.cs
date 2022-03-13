namespace HumidityService.Config;

public class ConsulPayload<T>
{
    public ConsulPayload(T settings)
    {
        this.HumiditySettings = settings;
    }
    
    public T HumiditySettings { get; set;}
}