using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace GlobalWeather.Client;

public class CustomAuthStateProvider(ILocalStorageService localStorage) : AuthenticationStateProvider
{
    private static IEnumerable<Claim> GetClaims(string token)
    {
        JwtSecurityTokenHandler handler = new();

        var read = handler.ReadJwtToken(token);

        return read.Claims;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await localStorage.GetItemAsStringAsync("token");

        var identity = new ClaimsIdentity("Bearer");

        if (!string.IsNullOrWhiteSpace(token))
        {
            identity.AddClaims(GetClaims(token));
        }
        
        var user = new ClaimsPrincipal(identity);
        var state = new AuthenticationState(user);

        NotifyAuthenticationStateChanged(Task.FromResult(state));

        return state;
    }
}