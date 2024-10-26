namespace GlobalWeather.Domain.Entities;

public class User
{
    public string Pk => Id;

    public string Sk => Pk;
    public string Id { get; init; } = default!;

    public string Email { get; set; } = default!;

    public byte[] PasswordHash { get; private set; } = [];

    public byte[] PasswordSalt { get; private set; } = [];

    public List<int> FavoriteCountries { get; set; } = [];

    public List<FavoriteCity> FavoriteCities { get; set; } = [];

    public void Update(string email)
    {
        Email = email.Trim();
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