using System.Net.Http.Json;
using GlobalWeather.Api.Lambda.Services;
using GlobalWeather.Shared.Models;
using GlobalWeather.Shared.Models.Users;
using Microsoft.Extensions.DependencyInjection;

namespace GlobalWeather.Integration.Tests.Endpoints;

public class UserEndpointsTests : IAsyncLifetime
{
    private readonly WebApplicationFactory _factory = new();

    [Fact]
    public async Task Should_Create_User()
    {
        using var client = _factory.CreateClient();

        var body = new CreateUserModel
        {
            Email = "test@test.com",
            Password = "test123456"
        };

        var post = await client.PostAsJsonAsync(
            "api/v1/user/create",
            body);

        var result = await post.Content.ReadFromJsonAsync<ResultModel<string>>();

        var claims = TokenService.GetClaims(result!.Result!);
        var id = claims.First(i => i.Type == "Id").Value;
        var getUser = await client.GetFromJsonAsync<ResultModel<UserModel>>(
            $"api/v1/user/{id}");

        Assert.True(post.IsSuccessStatusCode);
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.False(string.IsNullOrWhiteSpace(result.Result));

        Assert.NotNull(getUser);
        Assert.True(getUser.Success);
        Assert.Equal(getUser.Result!.Email, body.Email);
    }

    [Fact]
    public async Task Should_Perform_Login()
    {
        using var client = _factory.CreateClient();

        const string email = "login@test.com";
        const string password = "test123456";

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

        var result = await login.Content.ReadFromJsonAsync<ResultModel<string>>();
        var claims = TokenService.GetClaims(result!.Result!);

        Assert.True(login.IsSuccessStatusCode);
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(claims.First(i => i.Type == "Id").Value, user.Id);
    }

    [Fact]
    public async Task Should_Delete_User()
    {
        using var client = _factory.CreateClient();

        var user = await TestDatabaseHelper.CreateUserAsync(
            "login@test.com",
            "test123456",
            _factory);

        var delete = await client.DeleteFromJsonAsync<ResultModel<string>>($"api/v1/user/{user.Id}");

        Assert.NotNull(delete);
        Assert.NotNull(delete.Result);
        Assert.True(delete.Success);
        Assert.Equal(user.Id, delete.Result!);
    }

    [Fact]
    public async Task Should_Return_False_For_Invalid_Token()
    {
        var client = _factory.CreateClient();
        var post = await client.PostAsJsonAsync(
            "api/v1/user/validate",
            Guid.NewGuid().ToString());

        var result = await post.Content.ReadFromJsonAsync<ResultModel<bool>>();

        Assert.True(post.IsSuccessStatusCode);
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.False(result.Result);
    }
    
    [Fact]
    public async Task Should_Return_Tru_For_Valid_Token()
    {
        var service = _factory.Services.GetRequiredService<TokenService>();
        var token = service.CreateToken(
            Guid.NewGuid().ToString(),
            "test@email.com");
        
        var client = _factory.CreateClient();
        var post = await client.PostAsJsonAsync(
            "api/v1/user/validate",
            token);

        var result = await post.Content.ReadFromJsonAsync<ResultModel<bool>>();

        Assert.True(post.IsSuccessStatusCode);
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.True(result.Result);
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