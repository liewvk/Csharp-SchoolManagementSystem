
namespace SchoolManagementSystem;

public sealed record UserSession(
    int UserID,
    string Username,
    string FullName,
    string Role)
{
    public bool IsAdministrator =>
        Role == "Administrator";

    public bool IsTeacher =>
        Role == "Teacher";

    public bool IsClerk =>
        Role == "Clerk";

    public bool CanManageUsers =>
        IsAdministrator;

    public bool CanRegisterStudents =>
        IsAdministrator || IsClerk;

    public bool CanManageResults =>
        IsAdministrator || IsTeacher;
}

