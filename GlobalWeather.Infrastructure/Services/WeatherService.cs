using System.Net.Http.Json;
using GlobalWeather.Shared.Contracts;
using GlobalWeather.Shared.Models;
using GlobalWeather.Shared.Models.Weather;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GlobalWeather.Infrastructure.Services;

internal sealed class WeatherService(
    HttpClient httpClient,
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
                $"data/2.5/weather?lat={latitude}&lon={longitude}&appid={apiKey}",
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

    public Task<ResultModel<string>> AddCityToFavoritesAsync(
        double latitude,
        double longitude,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ResultModel<string>> RemoveCityFromFavoritesAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}