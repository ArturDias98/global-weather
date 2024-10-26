using Amazon.DynamoDBv2.DataModel;

namespace GlobalWeather.Domain.Entities;

[DynamoDBTable("User")]
public class User
{
    private const double COORDINATE_COMPARE_TOLERANCE = 0.001;

    [DynamoDBHashKey] public string Id { get; init; } = default!;

    [DynamoDBProperty] public string Email { get; set; } = default!;

    [DynamoDBProperty] public byte[] PasswordHash { get; private set; } = [];

    [DynamoDBProperty] public byte[] PasswordSalt { get; private set; } = [];

    [DynamoDBProperty] public List<int> FavoriteCountries { get; set; } = [];

    [DynamoDBProperty] public List<FavoriteCity> FavoriteCities { get; set; } = [];

    public void Update(string email)
    {
        Email = email.Trim();
    }

    public void AddCountry(int code)
    {
        if (FavoriteCountries.Contains(code))
        {
            return;
        }

        FavoriteCountries.Add(code);
    }

    public void RemoveCountry(int code)
    {
        FavoriteCountries.Remove(code);
    }

    public string AddCity(
        double latitude,
        double longitude)
    {
        if (FavoriteCities.Any(i =>
                Math.Abs(i.Latitude - latitude) < COORDINATE_COMPARE_TOLERANCE &&
                Math.Abs(i.Longitude - longitude) < COORDINATE_COMPARE_TOLERANCE))
        {
            return string.Empty;
        }

        var create = FavoriteCity.Create(latitude, longitude);
        FavoriteCities.Add(create);

        return create.Id;
    }

    public void RemoveCity(string id)
    {
        FavoriteCities.RemoveAll(i => i.Id == id);
    }

    public void UpdatePassword(
        byte[] passwordHash,
        byte[] passwordSalt)
    {
        if (passwordHash.Length == 0)
        {
            throw new ArgumentException("Password hash must not be empty", nameof(passwordHash));
        }

        if (passwordSalt.Length == 0)
        {
            throw new ArgumentException("Password salt must not be empty", nameof(passwordSalt));
        }

        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
    }

    public static User Create(
        string email,
        byte[] passwordHash,
        byte[] passwordSalt)
    {
        return new()
        {
            Id = Guid.NewGuid().ToString(),
            Email = email.Trim(),
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt
        };
    }
}

public class FavoriteCity
{
    public string Id { get; set; } = default!;
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public static FavoriteCity Create(
        double latitude,
        double longitude) => new()
    {
        Id = Guid.NewGuid().ToString(),
        Latitude = latitude,
        Longitude = longitude
    };
}