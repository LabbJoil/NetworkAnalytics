using System.Security.Cryptography;
using System.Text;

namespace NetworkAnalytics.Services.System;

internal class UserConverter
{
    private static readonly byte[] SaltPassword = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 };

    public static string HashPassword(string password)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(password, SaltPassword, 10000, HashAlgorithmName.SHA384);
        byte[] hash = pbkdf2.GetBytes(32);
        string hashPassword = Convert.ToBase64String(hash);
        return hashPassword;
    }

    public static string HashUserSession(string input)
    {
        byte[] hashBytes = SHA384.HashData(Encoding.UTF8.GetBytes(input));
        StringBuilder builder = new();
        for (int i = 0; i < hashBytes.Length; i++)
        {
            builder.Append(hashBytes[i].ToString("x2"));
        }
        return builder.ToString();
    }
}
