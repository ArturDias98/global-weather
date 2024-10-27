using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using GlobalWeather.Shared.Models;
using GlobalWeather.Shared.Models.Users;

namespace GlobalWeather.Integration.Tests.Endpoints;

public class CountryEndpoints: IAsyncLifetime
{
    private readonly WebApplicationFactory _factory = new();
    
    [Fact]
    public async Task Should_Not_Add_Country_To_Favorites_When_Non_Authorized()
    {
        using var client = _factory.CreateClient();
        
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            Guid.NewGuid().ToString());
        
        var addCountryResult = await client.PutAsync(
            $"api/v1/country/{55}",
            null);
        
        Assert.False(addCountryResult.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, addCountryResult.StatusCode);
    }
    
    [Fact]
    public async Task Should_Add_Country_To_Favorites()
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

        const int code = 55;
        var addCountryResult = await client.PutAsync(
            $"api/v1/country/{code}",
            null);
        
        var getUser = await client.GetFromJsonAsync<ResultModel<UserModel>>(
            $"api/v1/user/{user.Id}");
        
        Assert.True(addCountryResult.IsSuccessStatusCode);
        
        Assert.NotNull(getUser);
        Assert.NotNull(getUser.Result);
        Assert.Contains(code, getUser.Result.FavoriteCountries);
    }
    
    [Fact]
    public async Task Should_Not_Remove_Country_From_Favorites_When_Non_Authorized()
    {
        using var client = _factory.CreateClient();
        
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            Guid.NewGuid().ToString());
        
        var removeCountryResult = await client.DeleteAsync(
            $"api/v1/country/{55}");
        
        Assert.False(removeCountryResult.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, removeCountryResult.StatusCode);
    }
    
    [Fact]
    public async Task Should_Remove_Country_From_Favorites()
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

        const int code = 55;
        var removeCountryResult = await client.DeleteAsync(
            $"api/v1/country/{code}");
        
        var getUser = await client.GetFromJsonAsync<ResultModel<UserModel>>(
            $"api/v1/user/{user.Id}");
        
        Assert.True(removeCountryResult.IsSuccessStatusCode);
        
        Assert.NotNull(getUser);
        Assert.NotNull(getUser.Result);
        Assert.DoesNotContain(code, getUser.Result.FavoriteCountries);
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