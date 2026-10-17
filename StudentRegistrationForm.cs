
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace SchoolManagementSystem;

public class StudentRegistrationForm : Form
{
    private readonly UserSession _session;

    private readonly TextBox txtName = new();
    private readonly TextBox txtClass = new();
    private readonly TextBox txtPhone = new();
    private readonly DateTimePicker dtpBirth = new();
    private readonly DataGridView dgvStudents = new();

    private readonly Button btnAdd = new();
    private readonly Button btnUpdate = new();
    private readonly Button btnClear = new();

    private int? selectedStudentID;

    public StudentRegistrationForm(UserSession session)
    {
        if (!session.CanRegisterStudents)
            throw new UnauthorizedAccessException(
                "You cannot access student registration.");

        _session = session;
        InitializeInterface();
        LoadStudents();
    }

    private void InitializeInterface()
    {
        Text = "Student Registration";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(900, 630);
        Font = new Font("Segoe UI", 10);

        Label title = new()
        {
            Text = "Student Registration",
            Font = new Font("Segoe UI", 17, FontStyle.Bold),
            Location = new Point(25, 20),
            AutoSize = true
        };

        AddLabel("Student Name:", 30, 90);
        txtName.SetBounds(175, 85, 250, 30);

        AddLabel("Class:", 30, 135);
        txtClass.SetBounds(175, 130, 250, 30);

        AddLabel("Phone Number:", 455, 90);
        txtPhone.SetBounds(610, 85, 240, 30);

        AddLabel("Date of Birth:", 455, 135);
        dtpBirth.SetBounds(610, 130, 240, 30);
        dtpBirth.Format = DateTimePickerFormat.Short;
        dtpBirth.ShowCheckBox = true;
        dtpBirth.Checked = false;

        btnAdd.Text = "Register Student";
        btnAdd.SetBounds(175, 200, 170, 40);
        btnAdd.Click += BtnAdd_Click;

        btnUpdate.Text = "Update Student";
        btnUpdate.SetBounds(365, 200, 170, 40);
        btnUpdate.Click += BtnUpdate_Click;

        btnClear.Text = "Clear";
        btnClear.SetBounds(555, 200, 120, 40);
        btnClear.Click += (s, e) => ClearFields();

        dgvStudents.SetBounds(30, 275, 835, 320);
        dgvStudents.ReadOnly = true;
        dgvStudents.AllowUserToAddRows = false;
        dgvStudents.SelectionMode =
            DataGridViewSelectionMode.FullRowSelect;
        dgvStudents.MultiSelect = false;
        dgvStudents.AutoSizeColumnsMode =
            DataGridViewAutoSizeColumnsMode.Fill;

        dgvStudents.CellClick += DgvStudents_CellClick;

        Controls.AddRange(new Control[]
        {
            title, txtName, txtClass, txtPhone, dtpBirth,
            btnAdd, btnUpdate, btnClear, dgvStudents
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

    private void LoadStudents()
    {
        const string sql = """
            SELECT StudentID, StudentName,
                   ClassName, DateOfBirth, PhoneNumber
            FROM dbo.Students
            ORDER BY StudentID DESC
            """;

        using var connection = DatabaseHelper.GetConnection();
        using var adapter = new SqlDataAdapter(sql, connection);

        DataTable table = new();
        adapter.Fill(table);
        dgvStudents.DataSource = table;
    }

    private bool ValidateFields()
    {
        if (string.IsNullOrWhiteSpace(txtName.Text) ||
            string.IsNullOrWhiteSpace(txtClass.Text))
        {
            MessageBox.Show(
                "Student name and class are required.");
            return false;
        }

        return true;
    }

    private void AddParameters(SqlCommand command)
    {
        command.Parameters.AddWithValue(
            "@Name", txtName.Text.Trim());

        command.Parameters.AddWithValue(
            "@Class", txtClass.Text.Trim());

        command.Parameters.AddWithValue(
            "@Phone", string.IsNullOrWhiteSpace(txtPhone.Text)
                ? DBNull.Value
                : txtPhone.Text.Trim());

        command.Parameters.Add("@DOB", SqlDbType.Date).Value =
            dtpBirth.Checked
                ? dtpBirth.Value.Date
                : DBNull.Value;
    }

    private void BtnAdd_Click(object? sender, EventArgs e)
    {
        if (!_session.CanRegisterStudents ||
            !ValidateFields())
            return;

        try
        {
            const string sql = """
                INSERT INTO dbo.Students
                    (StudentName, ClassName,
                     DateOfBirth, PhoneNumber)
                VALUES
                    (@Name, @Class, @DOB, @Phone)
                """;

            using var connection = DatabaseHelper.GetConnection();
            using var command = new SqlCommand(sql, connection);

            AddParameters(command);

            connection.Open();
            command.ExecuteNonQuery();

            MessageBox.Show("Student registered successfully.");
            LoadStudents();
            ClearFields();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Registration Error");
        }
    }

    private void DgvStudents_CellClick(
        object? sender,
        DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0)
            return;

        DataGridViewRow row = dgvStudents.Rows[e.RowIndex];

        selectedStudentID = Convert.ToInt32(
            row.Cells["StudentID"].Value);

        txtName.Text =
            Convert.ToString(row.Cells["StudentName"].Value);

        txtClass.Text =
            Convert.ToString(row.Cells["ClassName"].Value);

        txtPhone.Text =
            Convert.ToString(row.Cells["PhoneNumber"].Value);

        object dob = row.Cells["DateOfBirth"].Value;

        if (dob != DBNull.Value && dob != null)
        {
            dtpBirth.Value = Convert.ToDateTime(dob);
            dtpBirth.Checked = true;
        }
        else
        {
            dtpBirth.Checked = false;
        }
    }

    private void BtnUpdate_Click(object? sender, EventArgs e)
    {
        if (!_session.CanRegisterStudents)
            return;

        if (selectedStudentID == null)
        {
            MessageBox.Show("Select a student to update.");
            return;
        }

        if (!ValidateFields())
            return;

        try
        {
            const string sql = """
                UPDATE dbo.Students
                SET StudentName = @Name,
                    ClassName = @Class,
                    DateOfBirth = @DOB,
                    PhoneNumber = @Phone
                WHERE StudentID = @StudentID
                """;

            using var connection = DatabaseHelper.GetConnection();
            using var command = new SqlCommand(sql, connection);

            AddParameters(command);

            command.Parameters.AddWithValue(
                "@StudentID", selectedStudentID.Value);

            connection.Open();
            command.ExecuteNonQuery();

            MessageBox.Show("Student updated successfully.");
            LoadStudents();
            ClearFields();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Update Error");
        }
    }

    private void ClearFields()
    {
        selectedStudentID = null;
        txtName.Clear();
        txtClass.Clear();
        txtPhone.Clear();
        dtpBirth.Checked = false;
        dgvStudents.ClearSelection();
    }
}

