using System.Net.Http.Json;
using GlobalWeather.Shared.Contracts;
using GlobalWeather.Shared.Models;
using GlobalWeather.Shared.Models.Countries;
using Microsoft.Extensions.Logging;

namespace GlobalWeather.Infrastructure.Services;

internal sealed class CountryService(
    HttpClient httpClient,
    ILogger<CountryService> logger) : ICountryService
{
    public async Task<ResultModel<List<CountryModel>>> GetCountriesByRegionAsync(
        string region,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await httpClient.GetFromJsonAsync<List<CountryModel>>(
                             $"region/{region.Trim()}",
                             cancellationToken)
                         ?? [];

            return ResultModel<List<CountryModel>>.SuccessResult(result);
        }
        catch (Exception e)
        {
            logger.LogError("Error on get countries for region {region}. Error: {message}", region, e.ToString());
            return ResultModel<List<CountryModel>>.ErrorResult("Could search countries by region");
        }
    }

    public Task<ResultModel<CountryModel>> GetCountryByCodeAsync(
        int code,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ResultModel<List<CountryModel>>> GetFavoriteCountriesAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ResultModel<int>> AddCountryToFavoritesAsync(
        int code,
        string userId,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ResultModel<int>> RemoveCountryFromFavoritesAsync(
        int code,
        string userId,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}