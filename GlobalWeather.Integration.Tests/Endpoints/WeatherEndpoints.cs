using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using GlobalWeather.Domain.Entities;
using GlobalWeather.Shared.Models;
using GlobalWeather.Shared.Models.Users;
using GlobalWeather.Shared.Models.Weather;

namespace GlobalWeather.Integration.Tests.Endpoints;

public class WeatherEndpoints : IAsyncLifetime
{
    private readonly WebApplicationFactory _factory = new();

    [Fact]
    public async Task Should_Not_Add_City_To_Favorites_When_Non_Authorized()
    {
        using var client = _factory.CreateClient();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            Guid.NewGuid().ToString());

        var result = await client.PutAsync(
            $"api/v1/weather",
            null);

        Assert.False(result.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task Should_Add_City_To_Favorites()
    {
        using var client = _factory.CreateClient();

        const string email = "login@testuser.com";
        const string password = "password123456";

        var user = await TestDatabaseHelper.CreateUserAsync(
            email,
            password,
            _factory);

        var login = await client.PostAsJsonAsync(
            "api/v1/user/login",
            new LoginModel
            {
                Email = email,
                Password = password
            });

        var loginContent = await login.Content.ReadFromJsonAsync<ResultModel<string>>();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            loginContent!.Result);

        var model = new AddWeatherModel
        {
            Name = "Test Name",
            Country = "Test Country",
            State = "Test State",
            Latitude = 0.1,
            Longitude = 0.5,
        };
        var addCityResult = await client.PutAsJsonAsync(
            $"api/v1/weather",
            model);

        var getUser = await client.GetFromJsonAsync<ResultModel<UserModel>>(
            $"api/v1/user/{user.Id}");

        Assert.True(addCityResult.IsSuccessStatusCode);

        Assert.NotNull(getUser);
        Assert.NotNull(getUser.Result);
        Assert.Equal(getUser.Result.FavoriteCities[0].Latitude, model.Latitude);
        Assert.Equal(getUser.Result.FavoriteCities[0].Longitude, model.Longitude);
    }

    [Fact]
    public async Task Should_Not_Remove_City_From_Favorites_When_Non_Authorized()
    {
        using var client = _factory.CreateClient();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            Guid.NewGuid().ToString());

        var result = await client.DeleteAsync(
            $"api/v1/weather/abcde");

        Assert.False(result.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task Should_Remove_City_From_Favorites()
    {
        using var client = _factory.CreateClient();

        const string email = "login@testuser.com";
        const string password = "password123456";

        var city = FavoriteCity.Create(
            "Test Name",
            "Test Country",
            "Test State",
            1.5,
            2.5);
        var user = await TestDatabaseHelper.CreateUserAsync(
            email,
            password,
            _factory,
            favoriteCities: [city]);

        var login = await client.PostAsJsonAsync(
            "api/v1/user/login",
            new LoginModel
            {
                Email = email,
                Password = password
            });

        var loginContent = await login.Content.ReadFromJsonAsync<ResultModel<string>>();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            loginContent!.Result);

        var removeCityResult = await client.DeleteAsync(
            $"api/v1/weather/{city.Id}");

        var getUser = await client.GetFromJsonAsync<ResultModel<UserModel>>(
            $"api/v1/user/{user.Id}");

        Assert.True(removeCityResult.IsSuccessStatusCode);

        Assert.NotNull(getUser);
        Assert.NotNull(getUser.Result);
        Assert.DoesNotContain(getUser.Result.FavoriteCities, i => i.Id == city.Id);
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