using Amazon.DynamoDBv2.DataModel;
using GlobalWeather.Shared.Comparers;

namespace GlobalWeather.Domain.Entities;

[DynamoDBTable("User")]
public class User
{
    private const double CoordinateCompareTolerance = 0.001;

    [DynamoDBHashKey] public string Id { get; init; } = default!;

    [DynamoDBProperty] public string Email { get; set; } = default!;

    [DynamoDBProperty] public byte[] PasswordHash { get; private set; } = [];

    [DynamoDBProperty] public byte[] PasswordSalt { get; private set; } = [];

    [DynamoDBProperty("FavoriteCountries")]
    public List<int> FavoriteCountries { get; set; } = [];

    [DynamoDBProperty("FavoriteCities")] public List<FavoriteCity> FavoriteCities { get; set; } = [];

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
        string name,
        string country,
        string state,
        double latitude,
        double longitude)
    {
        if (FavoriteCities.Any(i =>
                CoordinateComparer.CompareCoordinates(i.Latitude, i.Longitude, latitude, longitude)))
        {
            return string.Empty;
        }

        var create = FavoriteCity.Create(
            name,
            country,
            state,
            latitude,
            longitude);
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
    public string Name { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public static FavoriteCity Create(
        string name,
        string country,
        string state,
        double latitude,
        double longitude) => new()
    {
        Id = Guid.NewGuid().ToString(),
        Name = name,
        Country = country,
        State = state,
        Latitude = latitude,
        Longitude = longitude
    };
}