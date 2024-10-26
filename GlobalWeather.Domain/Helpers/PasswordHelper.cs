using System.Security.Cryptography;
using System.Text;

namespace GlobalWeather.Domain.Helpers;

internal static class PasswordHelper
{
    public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512();
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }

    public static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512(passwordSalt);
        var compute = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

        return compute.SequenceEqual(passwordHash);
    }
}