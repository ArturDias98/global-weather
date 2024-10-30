using GlobalWeather.Domain.Entities;
using GlobalWeather.Shared.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace GlobalWeather.Integration.Tests.Services;

public class WeatherServiceTests : IAsyncLifetime
{
    private readonly WebApplicationFactory _factory = new();

    [Fact]
    public async Task Should_Add_City_To_User_Favorites()
    {
        var createUser = await TestDatabaseHelper.CreateUserAsync(
            "user@email.com",
            "test123456",
            _factory);

        var service = _factory.Services.GetRequiredService<IWeatherService>();

        var result = await service.AddCityToFavoritesAsync(
            createUser.Id,
            "Test City",
            "Test Country",
            "Test State",
            0.5,
            0.5);

        var userService = _factory.Services.GetRequiredService<IUserService>();
        var user = await userService.GetUserByIdAsync(createUser.Id);

        Assert.True(result.Success);
        Assert.False(string.IsNullOrWhiteSpace(result.Result));
        Assert.True(user.Success);
        Assert.NotNull(user.Result);
        Assert.Contains(user.Result.FavoriteCities, i => i.Id == result.Result);
    }

    [Fact]
    public async Task Should_Not_Add_City_To_User_Favorites_When_Coordinates_Already_Exists()
    {
        var createUser = await TestDatabaseHelper.CreateUserAsync(
            "user@email.com",
            "test123456",
            _factory,
            favoriteCities:
            [
                new FavoriteCity
                {
                    Id = "test-id",
                    Name = "Test City",
                    Country = "Test Country",
                    State = "Test State",
                    Latitude = 0.1,
                    Longitude = -0.1,
                }
            ]);

        var service = _factory.Services.GetRequiredService<IWeatherService>();

        var result = await service.AddCityToFavoritesAsync(
            createUser.Id,
            "Test Add City",
            "Test Add Country",
            "Test Add State",
            0.1,
            -0.1);

        var userService = _factory.Services.GetRequiredService<IUserService>();
        var user = await userService.GetUserByIdAsync(createUser.Id);

        Assert.True(result.Success);
        Assert.True(string.IsNullOrWhiteSpace(result.Result));
        Assert.True(user.Success);
        Assert.NotNull(user.Result);
        Assert.Single(user.Result.FavoriteCities);
    }
    
    [Fact]
    public async Task Should_Remove_City_From_User_Favorites()
    {
        var createUser = await TestDatabaseHelper.CreateUserAsync(
            "user@email.com",
            "test123456",
            _factory,
            favoriteCities:
            [
                new FavoriteCity
                {
                    Id = "test-id",
                    Name = "Test City",
                    Country = "Test Country",
                    State = "Test State",
                    Latitude = 0.1,
                    Longitude = -0.1,
                }
            ]);

        var service = _factory.Services.GetRequiredService<IWeatherService>();

        var result = await service.RemoveCityFromFavoritesAsync(
            createUser.Id,
            "test-id");

        var userService = _factory.Services.GetRequiredService<IUserService>();
        var user = await userService.GetUserByIdAsync(createUser.Id);

        Assert.True(result.Success);
        Assert.False(string.IsNullOrWhiteSpace(result.Result));
        Assert.True(user.Success);
        Assert.NotNull(user.Result);
        Assert.Empty(user.Result.FavoriteCities);
    }

    public Task InitializeAsync()
    {
        return TestDatabaseHelper.MigrateDatabaseAsync(_factory);
    }

    public Task DisposeAsync()
    {
        return _factory.DisposeAsync();
    }
}