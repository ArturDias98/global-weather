using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using GlobalWeather.Shared.Contracts;
using GlobalWeather.Shared.Models;
using GlobalWeather.Shared.Models.Weather;

namespace GlobalWeather.Client.Services;

internal sealed class WeatherService(
    HttpClient client,
    ILocalStorageService localStorage,
    ILogger<WeatherService> logger) : IWeatherService
{
    public async Task<ResultModel<List<CityModel>>> GetCitiesByNameAsync(
        string cityName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await client.GetFromJsonAsync<ResultModel<List<CityModel>>>(
                $"city/{cityName}",
                cancellationToken) ?? ResultModel<List<CityModel>>.ErrorResult("Couldn't get cities");

            return result;
        }
        catch (Exception e)
        {
            logger.LogError("Error on get cities by name {name}. Error: {error}",
                cityName,
                e.ToString());

            return ResultModel<List<CityModel>>.ErrorResult("Internal server error");
        }
    }

    public async Task<ResultModel<CityModel>> GetCityInformationAsync(
        double latitude,
        double longitude,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var lat = latitude.ToString("F4", CultureInfo.InvariantCulture);
            var lon = longitude.ToString("F4", CultureInfo.InvariantCulture);
            var result = await client.GetFromJsonAsync<ResultModel<CityModel>>(
                $"city/info?lat={lat}&lon={lon}",
                cancellationToken) ?? ResultModel<CityModel>.ErrorResult("Couldn't get city information");

            return result;
        }
        catch (Exception e)
        {
            logger.LogError("Error on get city information for latitude {lat} and longitude {lon}. Error: {error}",
                latitude,
                longitude,
                e.ToString());

            return ResultModel<CityModel>.ErrorResult("Internal server error");
        }
    }

    public async Task<ResultModel<WeatherModel>> GetWeatherInformationAsync(
        double latitude,
        double longitude,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var lat = latitude.ToString("F4", CultureInfo.InvariantCulture);
            var lon = longitude.ToString("F4", CultureInfo.InvariantCulture);
            var result = await client.GetFromJsonAsync<ResultModel<WeatherModel>>(
                $"info?lat={lat}&lon={lon}",
                cancellationToken) ?? ResultModel<WeatherModel>.ErrorResult("Couldn't get weather information");

            return result;
        }
        catch (Exception e)
        {
            logger.LogError("Error on get weather information for latitude {lat} and longitude {lon}. Error: {error}",
                latitude,
                longitude,
                e.ToString());

            return ResultModel<WeatherModel>.ErrorResult("Internal server error");
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
            var token = await localStorage.GetItemAsync<string>("token", cancellationToken);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var result = await client.PutAsJsonAsync(
                "",
                new AddWeatherModel
                {
                    Name = name,
                    Country = country,
                    State = state,
                    Latitude = latitude,
                    Longitude = longitude
                },
                cancellationToken);

            var content = await result.Content.ReadFromJsonAsync<ResultModel<string>>(cancellationToken);

            return content ?? ResultModel<string>.ErrorResult("Couldn't add city to favorites");
        }
        catch (Exception e)
        {
            logger.LogError(
                "Error on add city with latitude {lat} and longitude {lon} to user {user} favorites. Error: {error}",
                latitude,
                longitude,
                userId,
                e.ToString());

            return ResultModel<string>.ErrorResult("Internal server error");
        }
    }

    public async Task<ResultModel<string>> RemoveCityFromFavoritesAsync(
        string userId,
        string id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var token = await localStorage.GetItemAsync<string>("token", cancellationToken);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var result = await client.DeleteAsync(
                id,
                cancellationToken);

            var content = await result.Content.ReadFromJsonAsync<ResultModel<string>>(cancellationToken);

            return content ?? ResultModel<string>.ErrorResult("Couldn't remove city from favorites");
        }
        catch (Exception e)
        {
            logger.LogError("Error on remove city {id} from user {user} favorites. Error: {error}",
                id,
                userId,
                e.ToString());

            return ResultModel<string>.ErrorResult("Internal server error");
        }
    }
}