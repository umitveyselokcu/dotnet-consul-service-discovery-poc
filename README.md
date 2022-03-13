## Service discovery with Consul

Run command `docker-compose up -d` 

You will find two Weather Service 'tokyo-weather-service' 'london-weather-service', a Humidity Service 'humidity-service', 'consul-service', 'consul-client' running on docker, all connected to bridge-network.

Navigate to [Consul Client UI / Services](http://localhost:8500/ui/dc1/services) on your local browser

All applications registered their 'address' 'port' 'name' information and health checks to Consul Services at application start up, that will allow microservices to find each other, also http and tcp health checks are good to have.
You can find http and TCP health status in link [health-checks](http://localhost:8500/ui/dc1/services/London/instances/consul-server/London/health-checks)


Registration: 
```
    var registration = new AgentServiceRegistration()
    {
        ID = appName,
        Name = appName,
        Address = host,
        Port = port,
        Checks = new AgentServiceCheck[] {apiHealth, tcpCheck}
    };

    logger.LogInformation("Registering with Consul");
    consulClient.Agent.ServiceDeregister(registration.ID).ConfigureAwait(true);
    consulClient.Agent.ServiceRegister(registration).ConfigureAwait(true);

```


To be able to call Humidity Service from Weather Services I am using `HumidityProxyService` which gets container name/IP/port of Humidity Service from Consul Services and sends http request using container name/IP/port.


Getting service information using consul client  in `IConsulRegistryService`
```
    public Uri? GetServiceUri(string serviceName)
    {
        var service = _consulClient.Health.Service(serviceName).Result;
        return service is {Response: {Length: > 0}} ? new Uri($"http://{service.Response[0].Service.Address}") : null;
    }
```
Registering HumidityProxyService using BaseAddress from Consul Services
```
    builder.Services.AddHttpClient<IHumidityProxyService, HumidityProxyService>((serviceProvider, client) =>
    {    
        var service = serviceProvider.GetService<IConsulRegistryService>();
        client.BaseAddress = service?.GetServiceUri("Humidity");
    });
```

Service Discovery can also be used with an api gateway like Ocelot.

## Distributed Config

Our applications use the Consul Key/Value store to simplify distributed Config management. 

You can change some settings on Consul Key/Value store and check api response to see how easy to change settings on runtime.

To try, you can change 'DetailedResponse' to true/false in [Humidity Api, Key/Value Store](http://localhost:8500/ui/dc1/kv/HumidityApi/Humidity/appsettings.json/edit) and see the change in 'humidity' in responses  [Tokyo Weather Api Basic Get request](http://localhost:60002/WeatherForecast) and [Humidity Api, Get](http://localhost:60010/WeatherForecast) and [London Weather Api, Get](http://localhost:60001/WeatherForecast) .


Seeding K/V store with dummy data on start up
```
    private static void SeedData(string? appName, IConsulClient consulClient)
    {
        var data = new ConsulPayload<WeatherForecastSettings>(new WeatherForecastSettings
        {
            AllCities = true, CityName = $"Weather for {appName}"
        });

        var stringMessage = JsonConvert.SerializeObject(data, Formatting.None);
        var bytes = Encoding.UTF8.GetBytes(stringMessage);

        consulClient.KV.Put(new KVPair($"WeatherApi/{appName}/appsettings.json") {Value = bytes}).Wait();
    }
```
Injecting Settings to ServiceCollection
```
     services.Configure<WeatherForecastSettings>(configuration.GetSection(nameof(WeatherForecastSettings)));
```
Than call it from constructor as you need
```
    IOptionsSnapshot<WeatherForecastSettings> _consulConfigs
```
#### Usage on .Net

We can use ```IOptions<T>``` to for configs you are not expecting to change, use ```IOptionsSnapsot<T>``` for configs to be consistent for the entirety of a request and use ```IOptionsMonitor<T>``` to get real time config values.





#### Links for Tokyo Weather API and Key/Value Store

[Tokyo Weather Api Basic Get request](http://localhost:60002/WeatherForecast)

[Tokyo Weather Api, Key/Value Store](http://localhost:8500/ui/dc1/kv/WeatherApi/Tokyo/appsettings.json/edit)

[Tokyo Weather Api, swagger](http://localhost:60002/swagger/index.html)

#### Links for London Weather API and Key/Value Store

[London Weather Api, Key/Value Store](http://localhost:8500/ui/dc1/kv/WeatherApi/London/appsettings.json/edit)

[London Weather Api, Get](http://localhost:60001/WeatherForecast)

[Humidity Api, swagger](http://localhost:60001/swagger/index.html)

### Links for Humidity API and Key/Value Store

[Humidity Api, Key/Value Store](http://localhost:8500/ui/dc1/kv/HumidityApi/Humidity/appsettings.json/edit)

[Humidity Api, Get](http://localhost:60010/WeatherForecast)

[Humidity Api, swagger](http://localhost:60010/swagger/index.html)


