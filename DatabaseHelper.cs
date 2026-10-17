
using Microsoft.Data.SqlClient;

namespace SchoolManagementSystem;

public static class DatabaseHelper
{
    private const string ConnectionString =
        @"Server=(localdb)\MSSQLLocalDB;" +
        @"Database=SchoolManagementDB;" +
        @"Integrated Security=True;" +
        @"TrustServerCertificate=True;";

    public static SqlConnection GetConnection()
    {
        return new SqlConnection(ConnectionString);
    }

    public static void InitializeUsers()
    {
        using var connection = GetConnection();
        connection.Open();

        const string sql = """
            SELECT COUNT(*)
            FROM dbo.Users
            WHERE Username = @Username
            """;

        var accounts = new[]
        {
            new
            {
                Username = "admin",
                FullName = "School Administrator",
                Role = "Administrator"
            },
            new
            {
                Username = "teacher",
                FullName = "Demo Teacher",
                Role = "Teacher"
            },
            new
            {
                Username = "clerk",
                FullName = "Demo Clerk",
                Role = "Clerk"
            }
        };

        foreach (var account in accounts)
        {
            using var check = new SqlCommand(sql, connection);

            check.Parameters.AddWithValue(
                "@Username", account.Username);

            int count = Convert.ToInt32(
                check.ExecuteScalar());

            if (count > 0)
                continue;

            // For local demonstration only.
            string password = account.Role switch
            {
                "Administrator" => "Admin@12345",
                "Teacher" => "Teacher@12345",
                _ => "Clerk@12345"
            };

            const string insertSql = """
                INSERT INTO dbo.Users
                (Username, PasswordHash, FullName, UserRole)
                VALUES
                (@Username, @Hash, @FullName, @Role)
                """;

            using var insert =
                new SqlCommand(insertSql, connection);

            insert.Parameters.AddWithValue(
                "@Username", account.Username);

            insert.Parameters.AddWithValue(
                "@Hash", PasswordHasher.HashPassword(password));

            insert.Parameters.AddWithValue(
                "@FullName", account.FullName);

            insert.Parameters.AddWithValue(
                "@Role", account.Role);

            insert.ExecuteNonQuery();
        }
    }
}
