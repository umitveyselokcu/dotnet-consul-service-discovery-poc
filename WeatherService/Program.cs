using WeatherService.Config;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseDistributedConsulConfig();

builder.Services.AddConsulConfig(builder.Configuration);

builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthorization();

app.UseConsul();

app.MapControllers();

app.Run();
