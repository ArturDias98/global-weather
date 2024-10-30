namespace GlobalWeather.Shared.Models.Weather;

public class AddWeatherModel
{
    public required string Name { get; set; }
    public required string Country { get; set; }
    public required string State { get; set; }
    public required double Latitude { get; set; }
    public required double Longitude { get; set; }
}