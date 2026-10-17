
using Microsoft.Data.SqlClient;

namespace SchoolManagementSystem;

public static class AuthService
{
    public static UserSession? Login(
        string username,
        string password)
    {
        if (string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrEmpty(password))
        {
            return null;
        }

        const string sql = """
            SELECT UserID, Username, FullName,
                   UserRole, PasswordHash
            FROM dbo.Users
            WHERE Username = @Username
              AND IsActive = 1
            """;

        using var connection = DatabaseHelper.GetConnection();
        using var command = new SqlCommand(sql, connection);

        command.Parameters.AddWithValue(
            "@Username", username.Trim());

        connection.Open();

        using var reader = command.ExecuteReader();

        if (!reader.Read())
            return null;

        string storedHash =
            reader.GetString(reader.GetOrdinal("PasswordHash"));

        if (!PasswordHasher.VerifyPassword(
            password, storedHash))
        {
            return null;
        }

        return new UserSession(
            reader.GetInt32(reader.GetOrdinal("UserID")),
            reader.GetString(reader.GetOrdinal("Username")),
            reader.GetString(reader.GetOrdinal("FullName")),
            reader.GetString(reader.GetOrdinal("UserRole"))
        );
    }
}

