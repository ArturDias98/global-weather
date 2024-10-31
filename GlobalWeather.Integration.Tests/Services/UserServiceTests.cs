using GlobalWeather.Shared.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace GlobalWeather.Integration.Tests.Services;

public class UserServiceTests : IAsyncLifetime
{
    private readonly WebApplicationFactory _factory = new();

    [Fact]
    public async Task Should_Create_User()
    {
        const string email = "email@email.com";

        var service = _factory.Services.GetRequiredService<IUserService>();
        var create = await service.CreateUserAsync(
            email,
            "test123456");

        var user = await service.GetUserByIdAsync(create.Result ?? string.Empty);

        Assert.True(create.Success);
        Assert.NotNull(create.Result);
        Assert.True(user.Success);
        Assert.NotNull(user.Result);
        Assert.Equal(email, user.Result.Email);
    }

    [Fact]
    public async Task Should_Not_Create_User_When_Email_Already_Exists()
    {
        const string email = "email@email.com";

        await TestDatabaseHelper.CreateUserAsync(
            email,
            "123456789",
            _factory);

        var service = _factory.Services.GetRequiredService<IUserService>();
        var create = await service.CreateUserAsync(
            email,
            "test123456");

        Assert.False(create.Success);
        Assert.Null(create.Result);
    }

    [Fact]
    public async Task Should_Delete_User()
    {
        var create = await TestDatabaseHelper.CreateUserAsync(
            "email@email.com",
            "password123456",
            _factory);

        var service = _factory.Services.GetRequiredService<IUserService>();
        var delete = await service.DeleteUserAsync(create.Id);

        var user = await service.GetUserByIdAsync(delete.Result ?? string.Empty);

        Assert.True(delete.Success);
        Assert.NotNull(delete.Result);
        Assert.Equal(delete.Result, create.Id);
        Assert.False(user.Success);
        Assert.Null(user.Result);
    }

    [Fact]
    public async Task Should_Login()
    {
        const string email = "email@email.com";
        const string password = "test123456";

        await TestDatabaseHelper.CreateUserAsync(
            email,
            password,
            _factory);

        var service = _factory.Services.GetRequiredService<IUserService>();
        var login = await service.LoginAsync(
            email,
            password);

        Assert.True(login.Success);
        Assert.False(string.IsNullOrWhiteSpace(login.Result));
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