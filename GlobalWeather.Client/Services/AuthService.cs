using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace GlobalWeather.Client.Services;

internal class AuthService(
    ILocalStorageService localStorage,
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
}