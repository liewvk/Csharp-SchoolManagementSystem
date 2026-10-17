
using System.Drawing;
using System.Windows.Forms;

namespace SchoolManagementSystem;

public class LoginForm : Form
{
    private readonly TextBox txtUsername = new();
    private readonly TextBox txtPassword = new();
    private readonly Button btnLogin = new();
    private readonly Button btnExit = new();
    private readonly Label lblMessage = new();

    public LoginForm()
    {
        InitializeInterface();
    }

    private void InitializeInterface()
    {
        Text = "School Management System - Login";
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(460, 370);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        BackColor = Color.WhiteSmoke;
        Font = new Font("Segoe UI", 10);

        Label lblTitle = new()
        {
            Text = "School Management System",
            Font = new Font("Segoe UI", 17, FontStyle.Bold),
            Location = new Point(35, 35),
            Size = new Size(400, 45),
            TextAlign = ContentAlignment.MiddleCenter
        };

        Label lblUsername = new()
        {
            Text = "Username",
            Location = new Point(65, 110),
            AutoSize = true
        };

        txtUsername.Location = new Point(180, 105);
        txtUsername.Size = new Size(210, 30);

        Label lblPassword = new()
        {
            Text = "Password",
            Location = new Point(65, 165),
            AutoSize = true
        };

        txtPassword.Location = new Point(180, 160);
        txtPassword.Size = new Size(210, 30);
        txtPassword.UseSystemPasswordChar = true;

        btnLogin.Text = "Login";
        btnLogin.Location = new Point(180, 225);
        btnLogin.Size = new Size(100, 38);
        btnLogin.BackColor = Color.SteelBlue;
        btnLogin.ForeColor = Color.White;
        btnLogin.FlatStyle = FlatStyle.Flat;
        btnLogin.Click += BtnLogin_Click;

        btnExit.Text = "Exit";
        btnExit.Location = new Point(290, 225);
        btnExit.Size = new Size(100, 38);
        btnExit.Click += (s, e) => Close();

        lblMessage.Location = new Point(45, 290);
        lblMessage.Size = new Size(370, 40);
        lblMessage.ForeColor = Color.DarkRed;
        lblMessage.TextAlign = ContentAlignment.MiddleCenter;

        Controls.AddRange(new Control[]
        {
            lblTitle,
            lblUsername,
            txtUsername,
            lblPassword,
            txtPassword,
            btnLogin,
            btnExit,
            lblMessage
        });

        AcceptButton = btnLogin;
        CancelButton = btnExit;
    }

    private void BtnLogin_Click(object? sender, EventArgs e)
    {
        try
        {
            btnLogin.Enabled = false;
            lblMessage.Text = "";

            UserSession? session = AuthService.Login(
                txtUsername.Text,
                txtPassword.Text);

            if (session == null)
            {
                lblMessage.Text =
                    "Invalid username or password.";
                txtPassword.Clear();
                txtPassword.Focus();
                return;
            }

            Hide();

            try
            {
                using MainForm mainForm = new(session);
                mainForm.ShowDialog();
            }
            finally
            {
                txtPassword.Clear();
                txtUsername.Clear();
                lblMessage.Text = "";
                Show();
                txtUsername.Focus();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Login could not be completed:\n" + ex.Message,
                "Login Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        finally
        {
            btnLogin.Enabled = true;
        }
    }
}

