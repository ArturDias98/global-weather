using System.Security.Claims;
using GlobalWeather.Shared.Contracts;
using GlobalWeather.Shared.Models;
using GlobalWeather.Shared.Models.Countries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace GlobalWeather.Api.Lambda.Extensions;

internal static class CountryExplorerEndpoints
{
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
            .WithDescription("Returns a list of countries filtered by a region")
            .WithTags("Country")
            .Produces<ResultModel<List<CountryModel>>>()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

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
            .WithDescription("Returns country with a specific code")
            .WithTags("Country")
            //.CacheOutput(cfg => cfg.Expire(TimeSpan.FromMinutes(2)))
            .Produces<ResultModel<CountryModel>>()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

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
            .WithDescription("Add country to user favorites and return country code.")
            .WithTags("Country")
            .Produces<ResultModel<int>>()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

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
            .WithDescription("Remove country from user favorites and return country code")
            .WithTags("Country")
            .Produces<ResultModel<int>>()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

        return app;
    }
}