using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace GlobalWeather.Client;

public static class ServerHelper
{
    private const string LocalServerUrl = "https://localhost:51343/";
    private const string RemoteServerUrl = "https://g7dyc2vjj5.execute-api.us-east-1.amazonaws.com/Prod/";

    public static string GetServerUrl(IWebAssemblyHostEnvironment environment)
    {
        return environment.IsDevelopment()
            ? LocalServerUrl
            : RemoteServerUrl;
    }
}