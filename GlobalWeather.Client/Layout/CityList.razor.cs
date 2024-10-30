using GlobalWeather.Shared.Models.Weather;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GlobalWeather.Client.Layout;

public partial class CityList : ComponentBase
{
    private async Task OnShowInfo(CityModel city)
    {
        var parameters = new DialogParameters<WeatherInfoDialog>
        {
            { x => x.City, city },
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

    [Inject] private IDialogService DialogService { get; set; } = null!;
    
    [Parameter] [EditorRequired] public List<CityModel> Cities { get; set; } = null!;
}