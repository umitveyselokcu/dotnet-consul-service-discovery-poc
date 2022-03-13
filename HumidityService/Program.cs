using HumidityService.Config;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseDistributedConsulConfig();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddConsulConfig(builder.Configuration);
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseConsul();
app.UseAuthorization();

app.MapControllers();

app.Run();
