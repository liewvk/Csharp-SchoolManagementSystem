
using System.Security.Cryptography;

namespace SchoolManagementSystem;

public static class PasswordHasher
{
    private const int Iterations = 210000;
    private const int SaltSize = 16;
    private const int HashSize = 32;

    public static string HashPassword(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            HashSize);

        return $"PBKDF2-SHA256${Iterations}$" +
               $"{Convert.ToBase64String(salt)}$" +
               $"{Convert.ToBase64String(hash)}";
    }

    public static bool VerifyPassword(
        string password,
        string storedPassword)
    {
        try
        {
            string[] parts = storedPassword.Split('$');

            if (parts.Length != 4 ||
                parts[0] != "PBKDF2-SHA256")
                return false;

            int iterations = int.Parse(parts[1]);

            if (iterations < 100000 || iterations > 1000000)
                return false;

            byte[] salt = Convert.FromBase64String(parts[2]);
            byte[] expectedHash =
                Convert.FromBase64String(parts[3]);

            if (salt.Length != SaltSize ||
                expectedHash.Length != HashSize)
                return false;

            byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                iterations,
                HashAlgorithmName.SHA256,
                expectedHash.Length);

            return CryptographicOperations.FixedTimeEquals(
                actualHash, expectedHash);
        }
        catch (FormatException)
        {
            return false;
        }
        catch (OverflowException)
        {
            return false;
        }
        catch (CryptographicException)
        {
            return false;
        }
    }
}

