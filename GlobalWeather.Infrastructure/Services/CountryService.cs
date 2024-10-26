using System.Net.Http.Json;
using GlobalWeather.Domain.Repositories;
using GlobalWeather.Shared.Contracts;
using GlobalWeather.Shared.Models;
using GlobalWeather.Shared.Models.Countries;
using Microsoft.Extensions.Logging;

namespace GlobalWeather.Infrastructure.Services;

internal sealed class CountryService(
    HttpClient httpClient,
    IUserRepository userRepository,
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

    public async Task<ResultModel<CountryModel>> GetCountryByCodeAsync(
        int code,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await httpClient.GetFromJsonAsync<CountryModel[]>(
                             $"alpha/{code}",
                             cancellationToken)
                         ?? [];

            var first = result.First();

            return ResultModel<CountryModel>.SuccessResult(first);
        }
        catch (Exception e)
        {
            logger.LogError("Error on get country with code {code}. Error: {message}", code, e.ToString());
            return ResultModel<CountryModel>.ErrorResult("Could search country by Code");
        }
    }

    public async Task<ResultModel<int>> AddCountryToFavoritesAsync(
        int code,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await userRepository.GetUserByIdAsync(userId, cancellationToken);
            user.AddCountry(code);
            
            await userRepository.SaveUserAsync(user, cancellationToken);
            
            return ResultModel<int>.SuccessResult(code);
        }
        catch (Exception e)
        {
            logger.LogError("Error on add country {code} to user {id}. Error: {message}",
                code, 
                userId,
                e.ToString());
            return ResultModel<int>.ErrorResult("Could not add country to favorites");
        }
    }

    public async Task<ResultModel<int>> RemoveCountryFromFavoritesAsync(
        int code,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await userRepository.GetUserByIdAsync(userId, cancellationToken);
            user.RemoveCountry(code);
            
            await userRepository.SaveUserAsync(user, cancellationToken);
            
            return ResultModel<int>.SuccessResult(code);
        }
        catch (Exception e)
        {
            logger.LogError("Error on remove country {code} from user {id}. Error: {message}",
                code, 
                userId,
                e.ToString());
            return ResultModel<int>.ErrorResult("Could not remove country from favorites");
        }
    }
}