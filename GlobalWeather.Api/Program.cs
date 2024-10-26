using GlobalWeather.Api.Extensions;
using GlobalWeather.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.MapCountryEndpoints()
    .MapWeatherEndpoints();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();