using System.Security.Claims;
using GlobalWeather.Shared.Contracts;
using GlobalWeather.Shared.Models;
using GlobalWeather.Shared.Models.Weather;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.OpenApi.Models;

namespace GlobalWeather.Api.Lambda.Extensions;

internal static class WeatherEndpoints
{
    private const string TagName = "Weather";

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
            .Produces<ResultModel<List<CityModel>>>(StatusCodes.Status200OK, "application/json")
            .Produces<ResultModel<List<CityModel>>>(StatusCodes.Status404NotFound, "application/json")
            .WithOpenApi(x => new OpenApiOperation(x)
            {
                Description = "Returns a list of cities filtered by name",
                Summary = "Get cities by name",
                Tags =
                [
                    new OpenApiTag
                    {
                        Name = TagName
                    }
                ],
                Parameters =
                [
                    new OpenApiParameter
                    {
                        Name = "name",
                        Description = "City name",
                        Required = true,
                        In = ParameterLocation.Path,
                        Examples = new Dictionary<string, OpenApiExample>
                        {
                            {
                                "London",
                                new OpenApiExample { Description = "Returns a list of cities with London name" }
                            },
                            {
                                "New York",
                                new OpenApiExample { Description = "Returns a list of cities with New York name" }
                            }
                        }
                    },
                ],
            });

        app.MapGet("api/v1/weather/city/info", async (
                [FromQuery(Name = "lat")] double latitude,
                [FromQuery(Name = "lon")] double longitude,
                [FromServices] IWeatherService service,
                CancellationToken cancellationToken) =>
            {
                var result = await service.GetCityInformationAsync(
                    latitude,
                    longitude,
                    cancellationToken);

                return result.Success
                    ? Results.Ok(result)
                    : Results.NotFound(result);
            })
            .WithName("get-city-information")
            .Produces<ResultModel<CityModel>>(StatusCodes.Status200OK, "application/json")
            .Produces<ResultModel<CityModel>>(StatusCodes.Status404NotFound, "application/json")
            .WithOpenApi(x => new OpenApiOperation(x)
            {
                Description = "Return city information filtered by coordinates",
                Summary = "Get city information by coordinates",
                Tags =
                [
                    new OpenApiTag
                    {
                        Name = TagName
                    }
                ],
                Parameters =
                [
                    new OpenApiParameter
                    {
                        Name = "lat",
                        Description = "City latitude coordinate",
                        Required = true,
                        In = ParameterLocation.Query,
                    },
                    new OpenApiParameter
                    {
                        Name = "lon",
                        Description = "City longitude coordinate",
                        Required = true,
                        In = ParameterLocation.Query,
                    }
                ],
            });

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
            .Produces<ResultModel<WeatherModel>>(StatusCodes.Status200OK, "application/json")
            .Produces<ResultModel<WeatherModel>>(StatusCodes.Status404NotFound, "application/json")
            .WithOpenApi(x => new OpenApiOperation(x)
            {
                Description = "Returns weather information for specified city",
                Summary = "Get weather information by coordinates",
                Tags =
                [
                    new OpenApiTag
                    {
                        Name = TagName
                    }
                ],
                Parameters =
                [
                    new OpenApiParameter
                    {
                        Name = "lat",
                        Description = "City latitude coordinate",
                        Required = true,
                        In = ParameterLocation.Query,
                    },
                    new OpenApiParameter
                    {
                        Name = "lon",
                        Description = "City longitude coordinate",
                        Required = true,
                        In = ParameterLocation.Query,
                    }
                ],
            });

        return app;
    }

    private static WebApplication MapPutEndpoints(this WebApplication app)
    {
        app.MapPut("api/v1/weather", async (
                [FromBody] AddWeatherModel model,
                [FromServices] IWeatherService service,
                [FromServices] IOutputCacheStore outputCacheStore,
                ClaimsPrincipal claims,
                CancellationToken cancellationToken) =>
            {
                var id = claims!.FindFirst(i => i.Type == "Id")!.Value;

                var result = await service.AddCityToFavoritesAsync(
                    id,
                    model.Name,
                    model.Country,
                    model.State,
                    model.Latitude,
                    model.Longitude,
                    cancellationToken);

                if (result.Success)
                {
                    await outputCacheStore.EvictByTagAsync(
                        "get-user",
                        cancellationToken);
                }

                return result.Success
                    ? Results.Ok(result)
                    : Results.NotFound(result);
            })
            .RequireAuthorization()
            .WithName("add-city-to-user-favorites")
            .Produces<ResultModel<string>>(StatusCodes.Status200OK, "application/json")
            .Produces<ResultModel<string>>(StatusCodes.Status404NotFound, "application/json")
            .WithOpenApi(x => new OpenApiOperation(x)
            {
                Description =
                    "Add city to user favorites and return city identifier. This endpoint requires authorization, please use endpoints api/v1/user/login or api/v1/user/create to get JWT Token.",
                Summary = "Add city to user favorites",
                Tags =
                [
                    new OpenApiTag
                    {
                        Name = TagName
                    }
                ]
            });

        return app;
    }

    private static WebApplication MapDeleteEndpoints(this WebApplication app)
    {
        app.MapDelete("api/v1/weather/{cityId}", async (
                [FromRoute(Name = "cityId")] string cityId,
                [FromServices] IWeatherService service,
                [FromServices] IOutputCacheStore outputCacheStore,
                ClaimsPrincipal claims,
                CancellationToken cancellationToken) =>
            {
                var id = claims!.FindFirst(i => i.Type == "Id")!.Value;

                var result = await service.RemoveCityFromFavoritesAsync(
                    id,
                    cityId,
                    cancellationToken);

                if (result.Success)
                {
                    await outputCacheStore.EvictByTagAsync(
                        "get-user",
                        cancellationToken);
                }

                return result.Success
                    ? Results.Ok(result)
                    : Results.NotFound(result);
            })
            .RequireAuthorization()
            .WithName("remove-city-from-user-favorites")
            .Produces<ResultModel<string>>(StatusCodes.Status200OK, "application/json")
            .Produces<ResultModel<string>>(StatusCodes.Status404NotFound, "application/json")
            .WithOpenApi(x => new OpenApiOperation(x)
            {
                Description =
                    "Remove city from user favorites. This endpoint requires authorization, please use endpoints api/v1/user/login or api/v1/user/create to get JWT Token.",
                Summary = "Remove city from user favorites and return city identifier",
                Tags =
                [
                    new OpenApiTag
                    {
                        Name = TagName
                    }
                ],
                Parameters =
                [
                    new OpenApiParameter
                    {
                        Name = "cityId",
                        Description = "City identifier",
                        Required = true,
                        In = ParameterLocation.Path
                    }
                ]
            });

        return app;
    }
}