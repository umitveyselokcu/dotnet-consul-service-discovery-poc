using Consul;

namespace WeatherService.Config;

public static class ConsulServiceRegistryExtension
{
    public static void AddConsulConfig(this IServiceCollection services)
    {
        services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
        {
            consulConfig.Address = new Uri(Environment.GetEnvironmentVariable("ConsulAddress") ?? string.Empty);
        }));
    }

    public static void UseConsul(this IApplicationBuilder app)
    {
        var consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();
        var logger = app.ApplicationServices.GetRequiredService<ILoggerFactory>().CreateLogger("AppExtensions");
        var lifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
        
        var appName= Environment.GetEnvironmentVariable("ServiceName");
        var host= Environment.GetEnvironmentVariable("ServiceHost");
        var port= Convert.ToInt32(Environment.GetEnvironmentVariable("ServicePort") ?? string.Empty);

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
            Checks = new AgentServiceCheck[] {apiHealth , tcpCheck}  
        };

        logger.LogInformation("Registering with Consul");
        consulClient.Agent.ServiceDeregister(registration.ID).ConfigureAwait(true);
        consulClient.Agent.ServiceRegister(registration).ConfigureAwait(true);
        
        lifetime.ApplicationStopping.Register(() =>
        {
            logger.LogInformation("Unregistering from Consul");
        });
    }
}
