using GlobalWeather.Client.Layout;
using GlobalWeather.Shared.Contracts;
using GlobalWeather.Shared.Models.Weather;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GlobalWeather.Client.Pages;

public partial class WeatherInfo : ComponentBase, IDisposable
{
    private readonly CancellationTokenSource _tokenSource = new();
    private List<CityModel> _cities = [];
    private string _name = string.Empty;
    private bool _loading;

    private async Task OnShowInfo(CityModel city)
    {
        var parameters = new DialogParameters<WeatherInfoDialog>
        {
            { x => x.City, city }
        };

        var options = new DialogOptions()
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Small,
            FullWidth = true
        };

        await DialogService.ShowAsync<WeatherInfoDialog>(
            "Clima",
            parameters,
            options);
    }

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
    [Inject] private IDialogService DialogService { get; set; } = null!;

    public void Dispose()
    {
        _tokenSource.Cancel();
    }
}