using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using Blazored.LocalStorage;
using GlobalWeather.Shared.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using static GlobalWeather.Client.ServerHelper;

namespace GlobalWeather.Client;

public class CustomAuthStateProvider(
    IHttpClientFactory factory,
    IWebAssemblyHostEnvironment environment,
    ILocalStorageService localStorage) : AuthenticationStateProvider
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
        var token = await localStorage.GetItemAsync<string>("token");

        var identity = new ClaimsIdentity();

        if (!string.IsNullOrWhiteSpace(token))
        {
            try
            {
                using var client = factory.CreateClient();
                client.BaseAddress = new Uri(GetServerUrl(environment));

                var post = await client.PostAsJsonAsync(
                    "api/v1/user/validate",
                    token);

                var result = await post.Content.ReadFromJsonAsync<ResultModel<bool>>()
                             ?? throw new Exception("Could not validate token");

                if (!result.Result)
                {
                    throw new Exception("Invalid token");
                }

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