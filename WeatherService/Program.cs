using WeatherService.Config;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddConsulConfig();

var app = builder.Build();

app.UseAuthorization();

app.UseConsul();

app.MapControllers();

app.Run();
