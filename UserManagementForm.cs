
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace SchoolManagementSystem;

public class UserManagementForm : Form
{
    private readonly UserSession _session;

    private readonly TextBox txtUsername = new();
    private readonly TextBox txtFullName = new();
    private readonly TextBox txtPassword = new();

    private readonly ComboBox cmbRole = new();
    private readonly DataGridView dgvUsers = new();

    private readonly Button btnAdd = new();
    private readonly Button btnToggle = new();
    private readonly Button btnResetPassword = new();

    public UserManagementForm(UserSession session)
    {
        if (!session.CanManageUsers)
            throw new UnauthorizedAccessException(
                "Administrator access is required.");

        _session = session;
        InitializeInterface();
        LoadUsers();
    }

    private void InitializeInterface()
    {
        Text = "User Management";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(850, 620);
        Font = new Font("Segoe UI", 10);

        Label title = new()
        {
            Text = "User Account Management",
            Font = new Font("Segoe UI", 17, FontStyle.Bold),
            Location = new Point(25, 20),
            AutoSize = true
        };

        AddLabel("Username:", 30, 85);
        txtUsername.SetBounds(160, 80, 250, 30);

        AddLabel("Full Name:", 30, 130);
        txtFullName.SetBounds(160, 125, 250, 30);

        AddLabel("Password:", 30, 175);
        txtPassword.SetBounds(160, 170, 250, 30);
        txtPassword.UseSystemPasswordChar = true;

        AddLabel("User Role:", 455, 85);
        cmbRole.SetBounds(570, 80, 210, 30);
        cmbRole.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbRole.Items.AddRange(
            new object[] { "Administrator", "Teacher", "Clerk" });
        cmbRole.SelectedIndex = 1;

        btnAdd.Text = "Add User";
        btnAdd.SetBounds(455, 130, 150, 40);
        btnAdd.Click += BtnAdd_Click;

        btnToggle.Text = "Activate / Disable";
        btnToggle.SetBounds(620, 130, 170, 40);
        btnToggle.Click += BtnToggle_Click;

        btnResetPassword.Text = "Reset Password";
        btnResetPassword.SetBounds(455, 185, 160, 40);
        btnResetPassword.Click += BtnResetPassword_Click;

        dgvUsers.SetBounds(30, 265, 790, 315);
        dgvUsers.ReadOnly = true;
        dgvUsers.AllowUserToAddRows = false;
        dgvUsers.SelectionMode =
            DataGridViewSelectionMode.FullRowSelect;
        dgvUsers.MultiSelect = false;
        dgvUsers.AutoSizeColumnsMode =
            DataGridViewAutoSizeColumnsMode.Fill;

        Controls.AddRange(new Control[]
        {
            title, txtUsername, txtFullName,
            txtPassword, cmbRole,
            btnAdd, btnToggle, btnResetPassword,
            dgvUsers
        });
    }

    private void AddLabel(string text, int x, int y)
    {
        Controls.Add(new Label
        {
            Text = text,
            Location = new Point(x, y),
            AutoSize = true
        });
    }

    private void LoadUsers()
    {
        const string sql = """
            SELECT UserID, Username, FullName,
                   UserRole, IsActive, CreatedAt
            FROM dbo.Users
            ORDER BY UserID
            """;

        using var connection = DatabaseHelper.GetConnection();
        using var adapter = new SqlDataAdapter(sql, connection);

        DataTable table = new();
        adapter.Fill(table);
        dgvUsers.DataSource = table;
    }

    private void BtnAdd_Click(object? sender, EventArgs e)
    {
        if (!_session.CanManageUsers)
            return;

        if (string.IsNullOrWhiteSpace(txtUsername.Text) ||
            string.IsNullOrWhiteSpace(txtFullName.Text) ||
            txtPassword.Text.Length < 12)
        {
            MessageBox.Show(
                "Enter a username and full name, and a " +
                "password of at least 12 characters.");
            return;
        }

        try
        {
            const string sql = """
                INSERT INTO dbo.Users
                    (Username, PasswordHash, FullName, UserRole)
                VALUES
                    (@Username, @Hash, @FullName, @Role)
                """;

            using var connection = DatabaseHelper.GetConnection();
            using var command = new SqlCommand(sql, connection);

            command.Parameters.AddWithValue(
                "@Username", txtUsername.Text.Trim());
            command.Parameters.AddWithValue(
                "@Hash",
                PasswordHasher.HashPassword(txtPassword.Text));
            command.Parameters.AddWithValue(
                "@FullName", txtFullName.Text.Trim());
            command.Parameters.AddWithValue(
                "@Role", cmbRole.SelectedItem!.ToString()!);

            connection.Open();
            command.ExecuteNonQuery();

            MessageBox.Show("User added successfully.");

            txtUsername.Clear();
            txtFullName.Clear();
            txtPassword.Clear();

            LoadUsers();
        }
        catch (SqlException ex) when (
            ex.Number == 2601 || ex.Number == 2627)
        {
            MessageBox.Show(
                "That username already exists.");
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error");
        }
    }

    private int? GetSelectedUserID()
    {
        if (dgvUsers.SelectedRows.Count == 0)
            return null;

        return Convert.ToInt32(
            dgvUsers.SelectedRows[0].Cells["UserID"].Value);
    }

    private void BtnToggle_Click(object? sender, EventArgs e)
    {
        if (!_session.CanManageUsers)
            return;

        int? userId = GetSelectedUserID();

        if (userId == null)
        {
            MessageBox.Show("Select a user.");
            return;
        }

        if (userId == _session.UserID)
        {
            MessageBox.Show(
                "You cannot disable your own account.");
            return;
        }

        try
        {
            const string sql = """
                UPDATE dbo.Users
                SET IsActive =
                    CASE WHEN IsActive = 1 THEN 0 ELSE 1 END
                WHERE UserID = @UserID
                """;

            using var connection = DatabaseHelper.GetConnection();
            using var command = new SqlCommand(sql, connection);

            command.Parameters.AddWithValue("@UserID", userId.Value);
            connection.Open();
            command.ExecuteNonQuery();

            LoadUsers();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error");
        }
    }

    private void BtnResetPassword_Click(
        object? sender, EventArgs e)
    {
        if (!_session.CanManageUsers)
            return;

        int? userId = GetSelectedUserID();

        if (userId == null || txtPassword.Text.Length < 12)
        {
            MessageBox.Show(
                "Select a user and enter a new password " +
                "of at least 12 characters.");
            return;
        }

        try
        {
            const string sql = """
                UPDATE dbo.Users
                SET PasswordHash = @Hash
                WHERE UserID = @UserID
                """;

            using var connection = DatabaseHelper.GetConnection();
            using var command = new SqlCommand(sql, connection);

            command.Parameters.AddWithValue(
                "@Hash",
                PasswordHasher.HashPassword(txtPassword.Text));
            command.Parameters.AddWithValue(
                "@UserID", userId.Value);

            connection.Open();
            command.ExecuteNonQuery();

            txtPassword.Clear();
            MessageBox.Show("Password reset successfully.");
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error");
        }
    }
}

