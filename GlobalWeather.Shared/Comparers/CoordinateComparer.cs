using GlobalWeather.Shared.Models.Users;
using GlobalWeather.Shared.Models.Weather;

namespace GlobalWeather.Shared.Comparers;

public static class CoordinateComparer
{
    private const double CoordinateCompareTolerance = 0.001;

    public static bool CompareCoordinates(
        double xLatitude,
        double xLongitude,
        double yLatitude,
        double yLongitude)
    {
        return Math.Abs(xLatitude - yLatitude) < CoordinateCompareTolerance &&
               Math.Abs(xLongitude - yLongitude) < CoordinateCompareTolerance;
    }

    public static bool CompareFavoriteCityCoordinates(
        this FavoriteCityModel favoriteCity,
        double latitude,
        double longitude)
    {
        return CompareCoordinates(
            favoriteCity.Latitude,
            favoriteCity.Longitude,
            latitude,
            longitude);
    }

    public static bool CompareCityCoordinates(
        this CityModel city,
        double latitude,
        double longitude)
    {
        return CompareCoordinates(
            city.Latitude,
            city.Longitude,
            latitude,
            longitude);
    }
}