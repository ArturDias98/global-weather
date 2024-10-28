using GlobalWeather.Client.Layout;
using GlobalWeather.Shared.Contracts;
using GlobalWeather.Shared.Models.Countries;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GlobalWeather.Client.Pages;

public partial class Country : IDisposable
{
    private readonly CancellationTokenSource _tokenSource = new();

    private List<CountryModel> _countries = [];
    private bool _loading;
    private string _region = string.Empty;

    private static string GetCountryImage(CountryModel country)
    {
        return country.Flags.TryGetValue("png", out var image)
            ? image
            : string.Empty;
    }

    private async Task OnClick(CountryModel model)
    {
        var parameters = new DialogParameters<CountryDialog>
        {
            { x => x.Model, model }
        };

        var options = new DialogOptions()
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Small,
            FullWidth = true
        };

        await DialogService.ShowAsync<CountryDialog>(
            "Informação",
            parameters,
            options);
    }

    private async Task OnLoadAsync(string region)
    {
        if (_region == region)
        {
            return;
        }

        _region = region;

        if (string.IsNullOrWhiteSpace(region))
        {
            _countries = [];
            return;
        }

        _loading = true;
        StateHasChanged();

        var result = await CountryService.GetCountriesByRegionAsync(
            _region,
            _tokenSource.Token);

        _countries = result.Result?
                         .OrderBy(i => i.Region)
                         .ThenBy(i => i.Name.CommonName)
                         .ToList()
                     ?? [];

        _loading = false;
        StateHasChanged();
    }

    [Inject] private ICountryService CountryService { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;

    public void Dispose()
    {
        _tokenSource.Cancel();
    }
}