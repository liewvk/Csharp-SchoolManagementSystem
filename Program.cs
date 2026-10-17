
using System;
using System.Windows.Forms;

namespace SchoolManagementSystem;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        try
        {
            // Create the three demonstration accounts
            // if they do not already exist.
            DatabaseHelper.InitializeUsers();

            // Start the application with the Login Form.
            Application.Run(new LoginForm());
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "The application could not start.\n\n" +
                ex.Message +
                "\n\nCheck that SchoolManagementDB exists " +
                "and SQL Server LocalDB is running.",
                "Startup Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
