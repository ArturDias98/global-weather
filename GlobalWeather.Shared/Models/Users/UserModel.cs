namespace GlobalWeather.Shared.Models.Users;

public class UserModel
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<int> FavoriteCountries { get; set; } = [];
    public List<FavoriteCityModel> FavoriteCities { get; set; } = [];
}

public class FavoriteCityModel
{
    public string Id { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}