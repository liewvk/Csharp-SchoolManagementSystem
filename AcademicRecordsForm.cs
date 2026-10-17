
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace SchoolManagementSystem;

public class AcademicRecordsForm : Form
{
    private readonly UserSession _session;

    private readonly ComboBox cmbStudents = new();
    private readonly TextBox txtSubject = new();
    private readonly NumericUpDown nudScore = new();
    private readonly DataGridView dgvResults = new();

    private readonly Button btnSave = new();
    private readonly Button btnRefresh = new();

    public AcademicRecordsForm(UserSession session)
    {
        if (!session.CanManageResults)
            throw new UnauthorizedAccessException(
                "Teacher or administrator access is required.");

        _session = session;
        InitializeInterface();
        LoadStudents();
        LoadResults();
    }

    private void InitializeInterface()
    {
        Text = "Student Academic Records";
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(920, 610);
        Font = new Font("Segoe UI", 10);

        Label title = new()
        {
            Text = "Academic Records Management",
            Font = new Font("Segoe UI", 17, FontStyle.Bold),
            Location = new Point(25, 20),
            AutoSize = true
        };

        AddLabel("Student:", 30, 95);
        cmbStudents.SetBounds(150, 90, 280, 30);
        cmbStudents.DropDownStyle = ComboBoxStyle.DropDownList;

        AddLabel("Subject:", 30, 145);
        txtSubject.SetBounds(150, 140, 280, 30);

        AddLabel("Score:", 475, 95);
        nudScore.SetBounds(565, 90, 140, 30);
        nudScore.Minimum = 0;
        nudScore.Maximum = 100;
        nudScore.DecimalPlaces = 2;

        btnSave.Text = "Save Result";
        btnSave.SetBounds(565, 145, 145, 40);
        btnSave.Click += BtnSave_Click;

        btnRefresh.Text = "Refresh";
        btnRefresh.SetBounds(730, 145, 130, 40);
        btnRefresh.Click += (s, e) =>
        {
            LoadStudents();
            LoadResults();
        };

        dgvResults.SetBounds(30, 225, 850, 345);
        dgvResults.ReadOnly = true;
        dgvResults.AllowUserToAddRows = false;
        dgvResults.SelectionMode =
            DataGridViewSelectionMode.FullRowSelect;
        dgvResults.MultiSelect = false;
        dgvResults.AutoSizeColumnsMode =
            DataGridViewAutoSizeColumnsMode.Fill;

        Controls.AddRange(new Control[]
        {
            title, cmbStudents, txtSubject, nudScore,
            btnSave, btnRefresh, dgvResults
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
            SELECT StudentID,
                   CONCAT(StudentName, ' - ', ClassName)
                       AS DisplayName
            FROM dbo.Students
            ORDER BY StudentName
            """;

        using var connection = DatabaseHelper.GetConnection();
        using var adapter = new SqlDataAdapter(sql, connection);

        DataTable table = new();
        adapter.Fill(table);

        cmbStudents.DataSource = table;
        cmbStudents.DisplayMember = "DisplayName";
        cmbStudents.ValueMember = "StudentID";
    }

    private void LoadResults()
    {
        const string sql = """
            SELECT r.ResultID,
                   s.StudentName,
                   s.ClassName,
                   r.SubjectName,
                   r.Score,
                   u.FullName AS UpdatedBy,
                   r.UpdatedAt
            FROM dbo.StudentResults r
            INNER JOIN dbo.Students s
                ON r.StudentID = s.StudentID
            INNER JOIN dbo.Users u
                ON r.UpdatedBy = u.UserID
            ORDER BY s.StudentName, r.SubjectName
            """;

        using var connection = DatabaseHelper.GetConnection();
        using var adapter = new SqlDataAdapter(sql, connection);

        DataTable table = new();
        adapter.Fill(table);

        dgvResults.DataSource = table;
    }

    private void BtnSave_Click(object? sender, EventArgs e)
    {
        if (!_session.CanManageResults)
            return;

        if (cmbStudents.SelectedValue == null ||
            cmbStudents.SelectedValue is DataRowView ||
            string.IsNullOrWhiteSpace(txtSubject.Text))
        {
            MessageBox.Show(
                "Select a student and enter a subject.");
            return;
        }

        try
        {
            int studentId =
                Convert.ToInt32(cmbStudents.SelectedValue);

            string subject = txtSubject.Text.Trim();

            const string sql = """
                BEGIN TRANSACTION;

                UPDATE dbo.StudentResults WITH (UPDLOCK, SERIALIZABLE)
                SET Score = @Score,
                    UpdatedBy = @UserID,
                    UpdatedAt = SYSUTCDATETIME()
                WHERE StudentID = @StudentID
                  AND SubjectName = @Subject;

                IF @@ROWCOUNT = 0
                BEGIN
                    INSERT INTO dbo.StudentResults
                        (StudentID, SubjectName, Score, UpdatedBy)
                    VALUES
                        (@StudentID, @Subject, @Score, @UserID);
                END;

                COMMIT TRANSACTION;
                """;

            using var connection = DatabaseHelper.GetConnection();
            using var command = new SqlCommand(sql, connection);

            command.Parameters.AddWithValue(
                "@StudentID", studentId);

            command.Parameters.AddWithValue(
                "@Subject", subject);

            var scoreParameter = command.Parameters.Add(
                "@Score", SqlDbType.Decimal);

            scoreParameter.Precision = 5;
            scoreParameter.Scale = 2;
            scoreParameter.Value = nudScore.Value;

            command.Parameters.AddWithValue(
                "@UserID", _session.UserID);

            connection.Open();
            command.ExecuteNonQuery();

            MessageBox.Show("Academic result saved.");

            txtSubject.Clear();
            nudScore.Value = 0;
            LoadResults();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Save Error");
        }
    }
}
