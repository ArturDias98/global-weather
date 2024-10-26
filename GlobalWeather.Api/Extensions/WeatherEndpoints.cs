using GlobalWeather.Shared.Contracts;
using GlobalWeather.Shared.Models;
using GlobalWeather.Shared.Models.Weather;
using Microsoft.AspNetCore.Mvc;

namespace GlobalWeather.Api.Extensions;

internal static class WeatherEndpoints
{
    public static WebApplication MapWeatherEndpoints(this WebApplication app)
    {
        return app.MapGetEndpoints();
    }

    private static WebApplication MapGetEndpoints(this WebApplication app)
    {
        app.MapGet("api/v1/weather/city/{name}", async (
                [FromRoute] string name,
                [FromServices] IWeatherService service,
                CancellationToken cancellationToken) =>
            {
                var result = await service.GetCitiesByNameAsync(
                    name,
                    cancellationToken);

                return result.Success
                    ? Results.Ok(result)
                    : Results.NotFound();
            })
            .WithName("get-cities-by-name")
            .WithDescription("Returns a list of cities filtered by name")
            .Produces<ResultModel<List<CityModel>>>()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        app.MapGet("api/v1/weather/info", async (
                [FromQuery(Name = "lat")] double latitude,
                [FromQuery(Name = "lon")] double longitude,
                [FromServices] IWeatherService service,
                CancellationToken cancellationToken) =>
            {
                var result = await service.GetWeatherInformationAsync(
                    latitude,
                    longitude,
                    cancellationToken);

                return result.Success
                    ? Results.Ok(result)
                    : Results.NotFound();
            })
            .WithName("get-weather-information")
            .WithDescription("Returns weather information for specified city")
            .Produces<ResultModel<WeatherModel>>()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}