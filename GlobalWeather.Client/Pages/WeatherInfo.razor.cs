using GlobalWeather.Shared.Contracts;
using GlobalWeather.Shared.Models.Weather;
using Microsoft.AspNetCore.Components;

namespace GlobalWeather.Client.Pages;

public partial class WeatherInfo : ComponentBase, IDisposable
{
    private readonly CancellationTokenSource _tokenSource = new();
    private List<CityModel> _cities = [];
    private string _name = string.Empty;
    private bool _loading;

    private async Task OnLoadAsync(string name)
    {
        if (_name == name)
        {
            return;
        }

        _name = name;

        if (string.IsNullOrWhiteSpace(name))
        {
            _cities = [];
            return;
        }

        _loading = true;
        StateHasChanged();

        var result = await WeatherService.GetCitiesByNameAsync(
            _name,
            _tokenSource.Token);

        _cities = result.Result?
                      .OrderBy(i => i.Country)
                      .ThenBy(i => i.State)
                      .ThenBy(i => i.Name)
                      .ToList()
                  ?? [];

        _loading = false;
        StateHasChanged();
    }

    [Inject] private IWeatherService WeatherService { get; set; } = null!;

    public void Dispose()
    {
        _tokenSource.Cancel();
    }
}