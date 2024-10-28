using System.Net.Http.Json;
using GlobalWeather.Shared.Contracts;
using GlobalWeather.Shared.Models;
using GlobalWeather.Shared.Models.Countries;

namespace GlobalWeather.Client.Services;

internal sealed class CountryService(
    HttpClient client,
    ILogger<CountryService> logger) : ICountryService
{
    public async Task<ResultModel<List<CountryModel>>> GetCountriesByRegionAsync(
        string region,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await client.GetFromJsonAsync<ResultModel<List<CountryModel>>>(
                $"region/{region}",
                cancellationToken) ?? ResultModel<List<CountryModel>>.ErrorResult("Couldn't get countries");

            return result;
        }
        catch (Exception e)
        {
            logger.LogError("Error on get countries by region {region}. Error: {error}",
                region,
                e.ToString());

            return ResultModel<List<CountryModel>>.ErrorResult("Internal server error");
        }
    }

    public async Task<ResultModel<CountryModel>> GetCountryByCodeAsync(
        int code,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await client.GetFromJsonAsync<ResultModel<CountryModel>>(
                $"code/{code}",
                cancellationToken) ?? ResultModel<CountryModel>.ErrorResult("Couldn't get countries");

            return result;
        }
        catch (Exception e)
        {
            logger.LogError("Error on get country by code {code}. Error: {error}",
                code,
                e.ToString());

            return ResultModel<CountryModel>.ErrorResult("Internal server error");
        }
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