using GlobalWeather.Shared.Models;
using GlobalWeather.Shared.Models.Weather;

namespace GlobalWeather.Shared.Contracts;

public interface IWeatherService
{
    Task<ResultModel<List<CityModel>>> GetCitiesByNameAsync(
        string cityName,
        CancellationToken cancellationToken = default);

    Task<ResultModel<WeatherModel>> GetWeatherInformationAsync(
        double latitude,
        double longitude,
        CancellationToken cancellationToken = default);

    Task<ResultModel<string>> AddCityToFavoritesAsync(
        string userId,
        double latitude,
        double longitude,
        CancellationToken cancellationToken = default);

    Task<ResultModel<string>> RemoveCityFromFavoritesAsync(
        string userId,
        string id,
        CancellationToken cancellationToken = default);
}