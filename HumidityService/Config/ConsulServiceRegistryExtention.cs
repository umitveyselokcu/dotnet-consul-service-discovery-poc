using System.Text;
using Consul;
using HumidityService.Dto;
using Newtonsoft.Json;
using Winton.Extensions.Configuration.Consul;

namespace HumidityService.Config;

public static class ConsulServiceRegistryExtension
{
    public static void AddConsulConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
        {
            consulConfig.Address = new Uri(Environment.GetEnvironmentVariable("ConsulAddress") ?? string.Empty);
        }));
        services.Configure<HumiditySettings>(configuration.GetSection(nameof(HumiditySettings)));
    }

    public static void UseConsul(this IApplicationBuilder app)
    {
        var consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();
        var logger = app.ApplicationServices.GetRequiredService<ILoggerFactory>().CreateLogger("AppExtensions");
        var lifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();

        var appName = Environment.GetEnvironmentVariable("ServiceName");
        var host = Environment.GetEnvironmentVariable("ServiceHost");
        var port = Convert.ToInt32(Environment.GetEnvironmentVariable("ServicePort") ?? string.Empty);

        var apiHealth = new AgentCheckRegistration()
        {
            HTTP = $"http://host.docker.internal:{port}/api/health/status",
            Notes = $"http://host.docker.internal:{port}/api/health/status Checks /health/status on localhost",
            Timeout = TimeSpan.FromSeconds(3),
            Interval = TimeSpan.FromSeconds(10)
        };
        var tcpCheck = new AgentCheckRegistration()
        {
            TCP = $"host.docker.internal:{port}",
            Notes = $"Runs a TCP check on port {port}",
            Timeout = TimeSpan.FromSeconds(2),
            Interval = TimeSpan.FromSeconds(5),
        };

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
        lifetime.ApplicationStopping.Register(() => { logger.LogInformation("Unregistering from Consul"); });

        SeedData(appName, consulClient);
    }
    
    public static void UseDistributedConsulConfig(this IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, configuration) =>
        {
            var consulUri = Environment.GetEnvironmentVariable("ConsulAddress");
            var serviceName = Environment.GetEnvironmentVariable("ServiceName");

            configuration.AddConsul($"HumidityApi/{serviceName}/appsettings.json", options =>
            {
                options.ConsulConfigurationOptions = cco =>
                {
                    if (consulUri != null) cco.Address = new Uri(consulUri);
                };
                options.Optional = true;
                options.PollWaitTime = TimeSpan.FromMinutes(1);
                options.ReloadOnChange = true;
                options.OnLoadException = cxt => { cxt.Ignore = true; };
                options.OnWatchException = cxt =>
                {
                    var exp = cxt.Exception;
                    return options.PollWaitTime;
                };
            });
        });
    }
    
    private static void SeedData(string? appName, IConsulClient consulClient)
    {
        var data = new ConsulPayload<HumiditySettings>(new HumiditySettings
        {
            DetailedResponse = true
        });

        var stringMessage = JsonConvert.SerializeObject(data, Formatting.None);
        var bytes = Encoding.UTF8.GetBytes(stringMessage);

        consulClient.KV.Put(new KVPair($"HumidityApi/{appName}/appsettings.json") {Value = bytes}).Wait();
    }
}