using GlobalWeather.Shared.Models.Countries;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GlobalWeather.Client.Layout;

public partial class CountryList : ComponentBase
{
    private static string GetCountryImage(CountryModel country)
    {
        return country.Flags.TryGetValue("png", out var image)
            ? image
            : string.Empty;
    }

    private void OnRemove(CountryModel country)
    {
        if(IsReadOnly)
            return;
        Countries.RemoveAll(i => i.Code == country.Code);
        StateHasChanged();
    }

    private void OnAdd(CountryModel country)
    {
        if(IsReadOnly)
            return;
        Countries.Add(country);
        StateHasChanged();
    }

    private async Task OnClick(CountryModel model)
    {
        var parameters = new DialogParameters<CountryDialog>
        {
            { x => x.Model, model },
            { x => x.OnRemoveCountry, new EventCallback<CountryModel>(null, OnRemove) },
            { x => x.OnAddCountry, new EventCallback<CountryModel>(null, OnAdd) }
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

    [Inject] private IDialogService DialogService { get; set; } = null!;

    [Parameter] public bool IsReadOnly { get; set; } = true;
    [Parameter] [EditorRequired] public List<CountryModel> Countries { get; set; } = null!;
}