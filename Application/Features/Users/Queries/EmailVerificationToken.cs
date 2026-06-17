using System.Security.Cryptography;
using System.Text;

namespace Application.Features.Users.Queries;

internal static class EmailVerificationToken
{
    public static string Generate()
    {
        var tokenBytes = RandomNumberGenerator.GetBytes(32);

        return Convert.ToBase64String(tokenBytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    public static string Hash(string token)
    {
        var tokenBytes = Encoding.UTF8.GetBytes(token);
        var hashBytes = SHA256.HashData(tokenBytes);

        return Convert.ToHexString(hashBytes);
    }
}
