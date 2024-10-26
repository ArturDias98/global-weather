using GlobalWeather.Shared.Contracts;
using GlobalWeather.Shared.Models;
using GlobalWeather.Shared.Models.Countries;
using Microsoft.AspNetCore.Mvc;

namespace GlobalWeather.Api.Extensions;

internal static class CountryExplorerEndpoints
{
    public static WebApplication MapCountryEndpoints(this WebApplication app)
    {
        return app
            .MapGetEndpoints();
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
                : Results.NotFound();
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
                    : Results.NotFound();
            })
            .WithName("get-country-by-code")
            .WithDescription("Returns country with a specific code")
            .WithTags("Country")
            .Produces<ResultModel<CountryModel>>()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
        
        return app;
    }
}