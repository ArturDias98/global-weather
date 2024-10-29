using GlobalWeather.Shared.Contracts;
using GlobalWeather.Shared.Models.Weather;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GlobalWeather.Client.Layout;

public partial class WeatherInfoDialog : ComponentBase
{
    private WeatherModel _model = new();
    private bool _loading = true;

    private async Task LoadWeatherAsync()
    {
        var result = await WeatherService.GetWeatherInformationAsync(
            City.Latitude,
            City.Longitude,
            Token);

        if (result.Success)
        {
            _model = result.Result!;
        }

        StateHasChanged();
    }

    [Inject] private IWeatherService WeatherService { get; set; } = null!;

    [Parameter][EditorRequired] public CityModel City { get; set; } = null!;
    [Parameter] public CancellationToken Token { get; set; }

    [CascadingParameter] private MudDialogInstance Dialog { get; set; } = null!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadWeatherAsync();

            _loading = false;
            StateHasChanged();
        }
    }
}