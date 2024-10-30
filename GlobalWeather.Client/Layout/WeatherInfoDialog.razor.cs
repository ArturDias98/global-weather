using GlobalWeather.Client.Services;
using GlobalWeather.Shared.Contracts;
using GlobalWeather.Shared.Models.Users;
using GlobalWeather.Shared.Models.Weather;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using GlobalWeather.Shared.Comparers;

namespace GlobalWeather.Client.Layout;

public partial class WeatherInfoDialog : ComponentBase
{
    private const double CoordinateCompareTolerance = 0.001;

    private WeatherModel _model = new();
    private UserModel? _userModel;
    private bool _loading = true;
    private bool _sending;

    private bool CanAddToFavorites => _userModel?.FavoriteCities.Any(i =>
        i.CompareFavoriteCityCoordinates(City.Latitude, City.Longitude)) == false;

    private async Task OnAddToFavorites()
    {
        _sending = true;
        StateHasChanged();

        var result = await WeatherService.AddCityToFavoritesAsync(
            "",
            City.Name,
            City.Country,
            City.State,
            City.Latitude,
            City.Longitude,
            Token);

        Snackbar.Clear();
        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomCenter;

        if (result.Success)
        {
            Snackbar.Add("Cidade adicionada aos favoritos", Severity.Success);
            
            var favorite = new FavoriteCityModel
            {
                Id = result.Result!,
                Name = City.Name,
                Country = City.Country,
                State = City.State,
                Latitude = City.Latitude,
                Longitude = City.Longitude
            };

            _userModel!.FavoriteCities.Add(favorite);
            await OnAddCity.InvokeAsync(favorite);
        }
        else
        {
            Snackbar.Add("Não foi possível adicionar a cidade aos favoritos", Severity.Error);
        }

        _sending = false;
        StateHasChanged();
    }

    private async Task OnRemoveFromFavorites()
    {
        _sending = true;
        StateHasChanged();

        var id = _userModel!.FavoriteCities.First(i =>
            i.CompareFavoriteCityCoordinates(City.Latitude, City.Longitude)).Id;

        var result = await WeatherService.RemoveCityFromFavoritesAsync(
            "",
            id,
            Token);

        Snackbar.Clear();
        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomCenter;

        if (result.Success)
        {
            Snackbar.Add("Cidade removida dos favoritos", Severity.Success);
            
            _userModel!.FavoriteCities.RemoveAll(i => i.Id == result.Result!);
            
            var favorite = new FavoriteCityModel
            {
                Id = result.Result!,
                Name = City.Name,
                Country = City.Country,
                State = City.State,
                Latitude = City.Latitude,
                Longitude = City.Longitude
            };
            
            await OnRemoveCity.InvokeAsync(favorite);
        }
        else
        {
            Snackbar.Add("Não foi possível remover a cidade dos favoritos", Severity.Error);
        }

        _sending = false;
        StateHasChanged();
    }

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

    private async Task LoadUserAsync()
    {
        _userModel = await AuthService.GetUserAsync(Token);
    }

    [Inject] private IWeatherService WeatherService { get; set; } = null!;
    [Inject] private AuthService AuthService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    [Parameter] [EditorRequired] public CityModel City { get; set; } = null!;
    [Parameter] public EventCallback<FavoriteCityModel> OnAddCity { get; set; }
    [Parameter] public EventCallback<FavoriteCityModel> OnRemoveCity { get; set; }
    [Parameter] public CancellationToken Token { get; set; }

    [CascadingParameter] private MudDialogInstance Dialog { get; set; } = null!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await Task.WhenAll(
                LoadWeatherAsync(),
                LoadUserAsync());

            _loading = false;
            StateHasChanged();
        }
    }
}