using System.Security.Claims;
using GlobalWeather.Shared.Contracts;
using GlobalWeather.Shared.Models;
using GlobalWeather.Shared.Models.Countries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.OpenApi.Models;

namespace GlobalWeather.Api.Lambda.Extensions;

internal static class CountryExplorerEndpoints
{
    private const string TagName = "Country";

    public static WebApplication MapCountryEndpoints(this WebApplication app)
    {
        return app
            .MapGetEndpoints()
            .MapPutEndpoints()
            .MapDeleteEndpoints();
    }

    private static WebApplication MapGetEndpoints(this WebApplication app)
    {
        app.MapGet("api/v1/country/region/{region}", async (
                [FromRoute] string region,
                [FromServices] ICountryService service,
                CancellationToken cancellationToken) =>
            {
                var result = await service.GetCountriesByRegionAsync(
                    region,
                    cancellationToken);

                return result.Success
                    ? Results.Ok(result)
                    : Results.NotFound(result);
            })
            .WithName("get-countries-by-region")
            .Produces<ResultModel<List<CountryModel>>>(StatusCodes.Status200OK, "application/json")
            .Produces<ResultModel<List<CountryModel>>>(StatusCodes.Status404NotFound, "application/json")
            .WithOpenApi(x => new OpenApiOperation(x)
            {
                Description = "List countries from specified region. Examples: Asia, Europe, North America",
                Summary = "Get a list of countries filtered by region",
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
                        Name = "region",
                        Description = "Search by country region",
                        Required = true,
                        In = ParameterLocation.Path,
                        Examples = new Dictionary<string, OpenApiExample>
                        {
                            { "Europe", new OpenApiExample { Description = "Returns a list of European countries" } },
                            { "Asia", new OpenApiExample { Description = "Returns a list of Asian countries" } }
                        }
                    },
                ],
            });

        app.MapGet("api/v1/country/code/{code:int}", async (
                [FromRoute] int code,
                [FromServices] ICountryService service,
                CancellationToken cancellationToken) =>
            {
                var result = await service.GetCountryByCodeAsync(
                    code,
                    cancellationToken);

                return result.Success
                    ? Results.Ok(result)
                    : Results.NotFound(result);
            })
            .WithName("get-country-by-code")
            .Produces<ResultModel<CountryModel>>(StatusCodes.Status200OK, "application/json")
            .Produces<ResultModel<CountryModel>>(StatusCodes.Status404NotFound, "application/json")
            .WithOpenApi(x => new OpenApiOperation(x)
            {
                Description = "Get country information by unique code",
                Summary = "Get country information by unique code",
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
                        Name = "code",
                        Description = "Country unique code (ccn3)",
                        Required = true,
                        In = ParameterLocation.Path
                    }
                ]
            });

        return app;
    }

    private static WebApplication MapPutEndpoints(this WebApplication app)
    {
        app.MapPut("api/v1/country/{code:int}", async (
                [FromRoute(Name = "code")] int code,
                [FromServices] ICountryService service,
                [FromServices] IOutputCacheStore outputCacheStore,
                ClaimsPrincipal claims,
                CancellationToken cancellationToken) =>
            {
                var id = claims!.FindFirst(i => i.Type == "Id")!.Value;

                var result = await service.AddCountryToFavoritesAsync(
                    code,
                    id,
                    cancellationToken);

                if (result.Success)
                {
                    await outputCacheStore.EvictByTagAsync(
                        "get-user",
                        cancellationToken);
                }

                return result.Success
                    ? Results.Ok(result)
                    : Results.BadRequest(result);
            })
            .RequireAuthorization()
            .WithName("add-country-to-user-favorites")
            .Produces<ResultModel<int>>(StatusCodes.Status200OK, "application/json")
            .Produces<ResultModel<int>>(StatusCodes.Status400BadRequest, "application/json")
            .Produces(StatusCodes.Status401Unauthorized)
            .WithOpenApi(x => new OpenApiOperation(x)
            {
                Description =
                    "Add country to user favorites. This endpoint requires authorization, please use endpoints api/v1/user/login or api/v1/user/create to get JWT Token.",
                Summary = "Add country to user favorites and return country code",
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
                        Name = "code",
                        Description = "Country unique code (ccn3)",
                        Required = true,
                        In = ParameterLocation.Path
                    }
                ]
            });

        return app;
    }

    private static WebApplication MapDeleteEndpoints(this WebApplication app)
    {
        app.MapDelete("api/v1/country/{code:int}", async (
                [FromRoute(Name = "code")] int code,
                [FromServices] ICountryService service,
                [FromServices] IOutputCacheStore outputCacheStore,
                ClaimsPrincipal claims,
                CancellationToken cancellationToken) =>
            {
                var id = claims!.FindFirst(i => i.Type == "Id")!.Value;

                var result = await service.RemoveCountryFromFavoritesAsync(
                    code,
                    id,
                    cancellationToken);

                if (result.Success)
                {
                    await outputCacheStore.EvictByTagAsync(
                        "get-user",
                        cancellationToken);
                }

                return result.Success
                    ? Results.Ok(result)
                    : Results.BadRequest(result);
            })
            .RequireAuthorization()
            .WithName("remove-country-from-user-favorites")
            .Produces<ResultModel<int>>(StatusCodes.Status200OK, "application/json")
            .Produces<ResultModel<int>>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithOpenApi(x => new OpenApiOperation(x)
            {
                Description =
                    "Remove country from user favorites. This endpoint requires authorization, please use endpoints api/v1/user/login or api/v1/user/create to get JWT Token.",
                Summary = "Remove country from user favorites and return country code",
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
                        Name = "code",
                        Description = "Country unique code (ccn3)",
                        Required = true,
                        In = ParameterLocation.Path
                    }
                ]
            });

        return app;
    }
}