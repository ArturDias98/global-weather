using System.Security.Claims;
using System.Text.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace GlobalWeather.Client;

public class CustomAuthStateProvider(ILocalStorageService localStorage) : AuthenticationStateProvider
{
    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes)
                            ?? throw new Exception("Could not deserialize JSON");
        return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString() ?? string.Empty));
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2:
                base64 += "==";
                break;
            case 3:
                base64 += "=";
                break;
        }

        return Convert.FromBase64String(base64);
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await localStorage.GetItemAsStringAsync("token");

        var identity = new ClaimsIdentity();

        if (!string.IsNullOrWhiteSpace(token))
        {
            try
            {
                identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "Bearer");
            }
            catch (Exception)
            {
                //
            }
        }

        var user = new ClaimsPrincipal(identity);
        var state = new AuthenticationState(user);

        NotifyAuthenticationStateChanged(Task.FromResult(state));

        return state;
    }
}