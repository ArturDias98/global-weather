using GlobalWeather.Shared.Contracts;
using Microsoft.AspNetCore.Components;

namespace GlobalWeather.Client.Pages;

public partial class Country
{
    [Inject] private ICountryService CountryService { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var result = await CountryService.GetCountriesByRegionAsync("europe");
    }
}