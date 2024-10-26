using GlobalWeather.Shared.Models;
using GlobalWeather.Shared.Models.Countries;

namespace GlobalWeather.Shared.Contracts;

public interface ICountryService
{
    Task<ResultModel<List<CountryModel>>> GetCountriesByRegionAsync(
        string region,
        CancellationToken cancellationToken = default);

    Task<ResultModel<CountryModel>> GetCountryByCodeAsync(
        int code,
        CancellationToken cancellationToken = default);

    Task<ResultModel<int>> AddCountryToFavoritesAsync(
        int code,
        string userId,
        CancellationToken cancellationToken = default);

    Task<ResultModel<int>> RemoveCountryFromFavoritesAsync(
        int code,
        string userId,
        CancellationToken cancellationToken = default);
}