using GlobalWeather.Shared.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace GlobalWeather.Integration.Tests.Services;

public class CountryServiceTests : IAsyncLifetime
{
    private readonly WebApplicationFactory _factory = new();

    [Fact]
    public async Task Should_Add_Country_To_User_Favorites()
    {
        var createUser = await TestDatabaseHelper.CreateUserAsync(
            "user@email.com",
            "test123456",
            _factory);

        var service = _factory.Services.GetRequiredService<ICountryService>();

        const int code = 25;

        var result = await service.AddCountryToFavoritesAsync(
            code,
            createUser.Id);

        var userService = _factory.Services.GetRequiredService<IUserService>();
        var user = await userService.GetUserByIdAsync(createUser.Id);

        Assert.True(result.Success);
        Assert.Equal(code, result.Result);
        Assert.NotNull(user.Result);
        Assert.Contains(code, user.Result.FavoriteCountries);
    }
    
    [Fact]
    public async Task Should_Not_Add_Country_To_User_Favorites_When_Already_Exists()
    {
        const int code = 50;
        var createUser = await TestDatabaseHelper.CreateUserAsync(
            "user@email.com",
            "test123456",
            _factory,
            favoriteCountries:[code, 25, 35]);

        var service = _factory.Services.GetRequiredService<ICountryService>();

        var result = await service.AddCountryToFavoritesAsync(
            code,
            createUser.Id);

        var userService = _factory.Services.GetRequiredService<IUserService>();
        var user = await userService.GetUserByIdAsync(createUser.Id);

        Assert.True(result.Success);
        Assert.Equal(code, result.Result);
        Assert.NotNull(user.Result);
        Assert.Single(user.Result.FavoriteCountries, i => i == code);
    }
    
    [Fact]
    public async Task Should_Remove_Country_From_User_Favorites()
    {
        const int code = 50;
        var createUser = await TestDatabaseHelper.CreateUserAsync(
            "user@email.com",
            "test123456",
            _factory,
            favoriteCountries:[code, 40, 80, 100]);

        var service = _factory.Services.GetRequiredService<ICountryService>();

        var result = await service.RemoveCountryFromFavoritesAsync(
            code,
            createUser.Id);

        var userService = _factory.Services.GetRequiredService<IUserService>();
        var user = await userService.GetUserByIdAsync(createUser.Id);

        Assert.True(result.Success);
        Assert.Equal(code, result.Result);
        Assert.NotNull(user.Result);
        Assert.DoesNotContain(code, user.Result.FavoriteCountries);
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