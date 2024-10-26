using System.Text.Json.Serialization;

namespace GlobalWeather.Shared.Models.Users;

public class CreateUserModel
{
    [JsonPropertyName("email")] public required string Email { get; set; } = string.Empty;
    [JsonPropertyName("password")] public required string Password { get; set; } = string.Empty;
}