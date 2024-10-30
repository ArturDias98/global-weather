using System.Net.Http.Headers;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using GlobalWeather.Shared.Contracts;
using GlobalWeather.Shared.Models;
using GlobalWeather.Shared.Models.Countries;

namespace GlobalWeather.Client.Services;

internal sealed class CountryService(
    HttpClient client,
    ILocalStorageService localStorage,
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

    public async Task<ResultModel<int>> AddCountryToFavoritesAsync(
        int code,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var token = await localStorage.GetItemAsync<string>("token", cancellationToken);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var result = await client.PutAsync(
                code.ToString(),
                null,
                cancellationToken);

            result.EnsureSuccessStatusCode();
            
            var content = await result.Content.ReadFromJsonAsync<ResultModel<int>>(cancellationToken);

            return content ?? ResultModel<int>.ErrorResult("Couldn't add country to favorites");
        }
        catch (Exception e)
        {
            logger.LogError("Error on add country {code} to user {user} favorites. Error: {error}",
                code,
                userId,
                e.ToString());

            return ResultModel<int>.ErrorResult("Internal server error");
        }
    }

    public async Task<ResultModel<int>> RemoveCountryFromFavoritesAsync(
        int code,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var token = await localStorage.GetItemAsync<string>("token", cancellationToken);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            var result = await client.DeleteAsync(
                code.ToString(),
                cancellationToken);

            var content = await result.Content.ReadFromJsonAsync<ResultModel<int>>(cancellationToken);

            return content ?? ResultModel<int>.ErrorResult("Couldn't remove country from favorites");
        }
        catch (Exception e)
        {
            logger.LogError("Error on remove country {code} from user {user} favorites. Error: {error}",
                code,
                userId,
                e.ToString());

            return ResultModel<int>.ErrorResult("Internal server error");
        }
    }
}