using Blazored.LocalStorage;
using GlobalWeather.Shared.Contracts;
using GlobalWeather.Shared.Models.Users;
using Microsoft.AspNetCore.Components.Authorization;

namespace GlobalWeather.Client.Services;

internal class AuthService(
    ILocalStorageService localStorage,
    IUserService userService,
    AuthenticationStateProvider authenticationStateProvider)
{
    public async Task AuthenticateAsync(
        string token,
        CancellationToken cancellationToken = default)
    {
        await localStorage.SetItemAsync(
            "token",
            token,
            cancellationToken);
        await authenticationStateProvider.GetAuthenticationStateAsync();
    }

    public async Task ClearAsync(CancellationToken cancellationToken = default)
    {
        await localStorage.RemoveItemAsync("token", cancellationToken);
        await authenticationStateProvider.GetAuthenticationStateAsync();
    }

    public async Task<UserModel?> GetUserAsync(CancellationToken cancellationToken = default)
    {
        var state = await authenticationStateProvider.GetAuthenticationStateAsync();
        var id = state.User.Claims.FirstOrDefault(i => i.Type == "Id")?.Value;

        if (string.IsNullOrWhiteSpace(id)) return null;

        var result = await userService.GetUserByIdAsync(id, cancellationToken);

        return result.Result;
    }
}