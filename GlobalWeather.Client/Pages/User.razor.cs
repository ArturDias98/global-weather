using GlobalWeather.Client.Layout;
using GlobalWeather.Client.Services;
using GlobalWeather.Shared.Contracts;
using GlobalWeather.Shared.Models.Countries;
using GlobalWeather.Shared.Models.Users;
using GlobalWeather.Shared.Models.Weather;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GlobalWeather.Client.Pages;

public partial class User : ComponentBase, IDisposable
{
    private readonly CancellationTokenSource _tokenSource = new();
    private bool _loading = true;
    private bool _loadingCountries = true;
    private UserModel? _user;
    private readonly List<CountryModel> _countries = [];

    private void OnRemove(FavoriteCityModel city)
    {
        _user!.FavoriteCities.RemoveAll(i => i.Id == city.Id);
        StateHasChanged();
    }

    private void OnAdd(FavoriteCityModel city)
    {
        _user!.FavoriteCities.Add(city);
        StateHasChanged();
    }

    private async Task OnShowInfo(CityModel city)
    {
        var parameters = new DialogParameters<WeatherInfoDialog>
        {
            { x => x.City, city },
            { x => x.OnAddCity, new EventCallback<FavoriteCityModel>(null, OnAdd) },
            { x => x.OnRemoveCity, new EventCallback<FavoriteCityModel>(null, OnRemove) }
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

    private async Task LoadCountries(List<int> countries)
    {
        foreach (var country in countries)
        {
            var result = await CountryService.GetCountryByCodeAsync(
                country,
                _tokenSource.Token);

            if (result.Success)
            {
                _countries.Add(result.Result!);
            }
        }

        _loadingCountries = false;
        StateHasChanged();
    }

    [Inject] private AuthService AuthService { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ICountryService CountryService { get; set; } = null!;
    [Inject] private IWeatherService WeatherService { get; set; } = null!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _user = await AuthService.GetUserAsync(_tokenSource.Token);

            if (_user is { } user)
            {
                LoadCountries(user.FavoriteCountries);
            }

            _loading = false;
            StateHasChanged();
        }
    }

    public void Dispose()
    {
        _tokenSource.Cancel();
    }
}