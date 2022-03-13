using WeatherService.Config;
using WeatherService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseDistributedConsulConfig();
builder.Services.AddConsulConfig(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<IHumidityProxyService, HumidityProxyService>((serviceProvider, client) =>
{
    var service = serviceProvider.GetService<IConsulRegistryService>();
    client.BaseAddress = service?.GetServiceUri("Humidity");
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthorization();

app.UseConsul();

app.MapControllers();

app.Run();
