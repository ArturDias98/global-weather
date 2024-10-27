using GlobalWeather.Shared.Contracts;
using GlobalWeather.Shared.Models;
using GlobalWeather.Shared.Models.Weather;
using Microsoft.AspNetCore.Mvc;

namespace GlobalWeather.Api.Extensions;

internal static class WeatherEndpoints
{
    public static WebApplication MapWeatherEndpoints(this WebApplication app)
    {
        return app
            .MapGetEndpoints()
            .MapPutEndpoints()
            .MapDeleteEndpoints();
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
                    : Results.NotFound(result);
            })
            .WithName("get-cities-by-name")
            .WithDescription("Returns a list of cities filtered by name")
            .WithTags("Weather")
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
                    : Results.NotFound(result);
            })
            .WithName("get-weather-information")
            .WithDescription("Returns weather information for specified city")
            .WithTags("Weather")
            .Produces<ResultModel<WeatherModel>>()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }

    private static WebApplication MapPutEndpoints(this WebApplication app)
    {
        app.MapPut("api/v1/weather/{userId}", async (
                [FromRoute] string userId,
                [FromBody] AddWeatherModel model,
                [FromServices] IWeatherService service,
                CancellationToken cancellationToken) =>
            {
                var result = await service.AddCityToFavoritesAsync(
                    userId,
                    model.Latitude,
                    model.Longitude,
                    cancellationToken);

                return result.Success
                    ? Results.Ok(result)
                    : Results.NotFound(result);
            })
            .WithName("add-city-to-user-favorites")
            .WithDescription("Add city to user favorites and return city identifier")
            .WithTags("Weather")
            .Produces<ResultModel<string>>()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }

    private static WebApplication MapDeleteEndpoints(this WebApplication app)
    {
        app.MapDelete("api/v1/weather/{userId}/{cityId}", async (
                [FromRoute(Name = "userId")] string userId,
                [FromRoute(Name = "cityId")] string cityId,
                [FromServices] IWeatherService service,
                CancellationToken cancellationToken) =>
            {
                var result = await service.RemoveCityFromFavoritesAsync(
                    userId,
                    cityId,
                    cancellationToken);

                return result.Success
                    ? Results.Ok(result)
                    : Results.NotFound(result);
            })
            .WithName("remove-city-from-user-favorites")
            .WithDescription("Remove city from user favorites and return city identifier")
            .WithTags("Weather")
            .Produces<ResultModel<string>>()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}