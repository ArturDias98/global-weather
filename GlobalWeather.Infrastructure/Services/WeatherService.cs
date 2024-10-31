using System.Net.Http.Json;
using GlobalWeather.Domain.Repositories;
using GlobalWeather.Shared.Comparers;
using GlobalWeather.Shared.Contracts;
using GlobalWeather.Shared.Models;
using GlobalWeather.Shared.Models.Weather;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GlobalWeather.Infrastructure.Services;

internal sealed class WeatherService(
    HttpClient httpClient,
    IUserRepository userRepository,
    IConfiguration configuration,
    ILogger<WeatherService> logger) : IWeatherService
{
    public async Task<ResultModel<List<CityModel>>> GetCitiesByNameAsync(
        string cityName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var apiKey = configuration.GetValue<string>("OpenWeatherMapApiKey")
                         ?? throw new Exception("Could not resolve OpenWeatherMapApiKey");

            var result = await httpClient.GetFromJsonAsync<List<CityModel>>(
                $"geo/1.0/direct?q={cityName}&limit=10&appid={apiKey}",
                cancellationToken) ?? [];

            return ResultModel<List<CityModel>>.SuccessResult(result);
        }
        catch (Exception e)
        {
            logger.LogError("Error on get cities by name {name}. Error: {error}",
                cityName,
                e.ToString());

            return ResultModel<List<CityModel>>.ErrorResult("Could not get cities by name");
        }
    }

    public async Task<ResultModel<CityModel>> GetCityInformationAsync(
        double latitude,
        double longitude,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var apiKey = configuration.GetValue<string>("OpenWeatherMapApiKey")
                         ?? throw new Exception("Could not resolve OpenWeatherMapApiKey");

            var result = await httpClient.GetFromJsonAsync<List<CityModel>>(
                $"geo/1.0/reverse?lat={latitude}&lon={longitude}&limit=10&appid={apiKey}",
                cancellationToken) ?? [];

            var first = result.FirstOrDefault();

            return first is not null
                ? ResultModel<CityModel>.SuccessResult(first)
                : ResultModel<CityModel>.ErrorResult("Could not find city");
        }
        catch (Exception e)
        {
            logger.LogError("Error on get cities by coordinates: lat - {lat} lon - {lon}. Error: {error}",
                latitude,
                longitude,
                e.ToString());

            return ResultModel<CityModel>.ErrorResult("Could not get city by coordinates");
        }
    }

    public async Task<ResultModel<WeatherModel>> GetWeatherInformationAsync(
        double latitude,
        double longitude,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var apiKey = configuration.GetValue<string>("OpenWeatherMapApiKey")
                         ?? throw new Exception("Could not resolve OpenWeatherMapApiKey");

            var result = await httpClient.GetFromJsonAsync<WeatherModel>(
                $"data/2.5/weather?lat={latitude}&lon={longitude}&appid={apiKey}&units=metric",
                cancellationToken) ?? new WeatherModel();

            return ResultModel<WeatherModel>.SuccessResult(result);
        }
        catch (Exception e)
        {
            logger.LogError(
                "Error on get weather information for coordinates lat={latitude}, lon={longitude}. Error: {error}",
                latitude,
                longitude,
                e.ToString());

            return ResultModel<WeatherModel>.ErrorResult("Could not get weather data");
        }
    }

    public async Task<ResultModel<string>> AddCityToFavoritesAsync(
        string userId,
        string name,
        string country,
        string state,
        double latitude,
        double longitude,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await userRepository.GetUserByIdAsync(userId, cancellationToken);
            
            if (user is null)
            {
                return ResultModel<string>.ErrorResult("User not found");
            }
            
            var id = user.AddCity(
                name,
                country,
                state,
                latitude,
                longitude);

            await userRepository.SaveUserAsync(user, cancellationToken);

            return ResultModel<string>.SuccessResult(id);
        }
        catch (Exception e)
        {
            logger.LogError("Error on add city {lat} - {lon} to user {id}. Error: {message}",
                latitude,
                longitude,
                userId,
                e.ToString());
            return ResultModel<string>.ErrorResult("Could not add city to favorites");
        }
    }

    public async Task<ResultModel<string>> RemoveCityFromFavoritesAsync(
        string userId,
        string id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await userRepository.GetUserByIdAsync(userId, cancellationToken);
            
            if (user is null)
            {
                return ResultModel<string>.ErrorResult("User not found");
            }
            
            user.RemoveCity(id);

            await userRepository.SaveUserAsync(user, cancellationToken);

            return ResultModel<string>.SuccessResult(id);
        }
        catch (Exception e)
        {
            logger.LogError("Error on remove city {id} to user {id}. Error: {message}",
                id,
                userId,
                e.ToString());
            return ResultModel<string>.ErrorResult("Could not remove city from favorites");
        }
    }
}