using Consul;

namespace WeatherService.Services;

public interface IConsulRegistryService
{
    Uri? GetServiceUri(string serviceName);
}

public class ConsulRegistryService : IConsulRegistryService
{
    private readonly IConsulClient _consulClient;

    public ConsulRegistryService(IConsulClient consulClient)
    {
        _consulClient = consulClient;
    }

    public Uri? GetServiceUri(string serviceName)
    {
        var service = _consulClient.Health.Service(serviceName).Result;
        return service is {Response: {Length: > 0}} ? new Uri($"http://{service.Response[0].Service.Address}") : null;
    }


}