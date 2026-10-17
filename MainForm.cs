
using System.Drawing;
using System.Windows.Forms;

namespace SchoolManagementSystem;

public class MainForm : Form
{
    private readonly UserSession _session;

    private readonly Button btnUsers = new();
    private readonly Button btnStudents = new();
    private readonly Button btnResults = new();
    private readonly Button btnLogout = new();

    public MainForm(UserSession session)
    {
        _session = session;
        InitializeInterface();
        ApplyPermissions();
    }

    private void InitializeInterface()
    {
        Text = "School Management System - Dashboard";
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(700, 450);
        BackColor = Color.WhiteSmoke;
        Font = new Font("Segoe UI", 10);

        Label lblTitle = new()
        {
            Text = "School Management Dashboard",
            Font = new Font("Segoe UI", 19, FontStyle.Bold),
            Location = new Point(50, 30),
            Size = new Size(600, 45),
            TextAlign = ContentAlignment.MiddleCenter
        };

        Label lblWelcome = new()
        {
            Text = $"Welcome, {_session.FullName}",
            Font = new Font("Segoe UI", 13),
            Location = new Point(70, 105),
            AutoSize = true
        };

        Label lblRole = new()
        {
            Text = $"Your Role: {_session.Role}",
            Location = new Point(70, 145),
            AutoSize = true,
            ForeColor = Color.DarkBlue
        };

        ConfigureButton(
            btnUsers, "Manage Users", 70, 215);

        ConfigureButton(
            btnStudents, "Student Registration", 270, 215);

        ConfigureButton(
            btnResults, "Academic Records", 470, 215);

        btnLogout.Text = "Logout";
        btnLogout.Location = new Point(270, 330);
        btnLogout.Size = new Size(150, 42);
        btnLogout.BackColor = Color.IndianRed;
        btnLogout.ForeColor = Color.White;
        btnLogout.FlatStyle = FlatStyle.Flat;

        btnUsers.Click += (s, e) =>
        {
            if (!_session.CanManageUsers)
                return;

            using UserManagementForm form = new(_session);
            form.ShowDialog(this);
        };

        btnStudents.Click += (s, e) =>
        {
            if (!_session.CanRegisterStudents)
                return;

            using StudentRegistrationForm form = new(_session);
            form.ShowDialog(this);
        };

        btnResults.Click += (s, e) =>
        {
            if (!_session.CanManageResults)
                return;

            using AcademicRecordsForm form = new(_session);
            form.ShowDialog(this);
        };

        btnLogout.Click += (s, e) => Close();

        Controls.AddRange(new Control[]
        {
            lblTitle,
            lblWelcome,
            lblRole,
            btnUsers,
            btnStudents,
            btnResults,
            btnLogout
        });
    }

    private void ConfigureButton(
        Button button,
        string caption,
        int x,
        int y)
    {
        button.Text = caption;
        button.Location = new Point(x, y);
        button.Size = new Size(160, 70);
        button.BackColor = Color.SteelBlue;
        button.ForeColor = Color.White;
        button.FlatStyle = FlatStyle.Flat;
    }

    private void ApplyPermissions()
    {
        btnUsers.Enabled = _session.CanManageUsers;
        btnStudents.Enabled = _session.CanRegisterStudents;
        btnResults.Enabled = _session.CanManageResults;
    }
}

