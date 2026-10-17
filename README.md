# C# School Management System

Csharp Database Programming Made Easy — Chapter 21

This repository contains the sample School Management System project that accompanies "Csharp Database Programming Made Easy" (Chapter 21). It's a C# desktop/web console application demonstrating database-driven CRUD operations for managing students, courses, enrollments, and staff in a simple school environment.

> Note: This README is written to be repository-agnostic because the original project may target different runtime versions or database engines. If you know the exact target (for example: .NET Framework 4.x / .NET 6 / SQL Server 2019), update the prerequisites and connection string examples accordingly.

## Table of contents

- Project overview
- Features
- Prerequisites
- Database setup
- Configuration
- Build and run
- Project structure
- Usage examples
- Tests (if any)
- Contributing
- Troubleshooting
- License
- Author / Contact

## Project overview

This sample application demonstrates:

- Designing a simple relational model for a school (Students, Courses, Enrollments, Teachers/Staff)
- Performing CRUD (Create, Read, Update, Delete) operations from C# against a relational database
- Using ADO.NET (SqlClient/DbProviderFactory) or an ORM like Entity Framework (if included) to interact with the database
- Basic validation, error handling, and transactional operations

The code in this repository is intended for educational purposes and to illustrate patterns from the book chapter.

## Features

- Manage Students: add, update, list, delete
- Manage Courses: add, update, list, delete
- Enroll students in courses and manage enrollments
- Query examples: list students by course, courses for a student, enrollment counts
- Example database scripts for schema creation and sample data (if included)

## Prerequisites

- .NET SDK or Visual Studio
  - If the project targets .NET Framework: Visual Studio 2019/2022 with the appropriate targeting packs
  - If the project targets .NET (Core/6/7): .NET 6.0 SDK or newer
- A relational database engine. Common examples:
  - Microsoft SQL Server (Express/Developer/Instance)
  - SQLite (for a lightweight file-based DB)
  - PostgreSQL/MySQL (if code has been adapted)
- (Optional) Entity Framework Core or other ORM packages if the project uses them

Install tools:

- Git (to clone the repo)
- Visual Studio / Visual Studio Code or your preferred IDE

## Database setup

There are two common approaches depending on what’s provided in the repository:

1) SQL script provided in the repo

- Look for a folder named `sql`, `scripts`, or `Database` with files like `create_schema.sql` or `seed_data.sql`.
- Run the schema script in your database (SQL Server Management Studio for SQL Server or the appropriate CLI for your DB).

2) Code-first (migrations)

- If the project uses Entity Framework with migrations, run the migrations from the Package Manager Console or dotnet CLI:

  - Package Manager Console (Visual Studio):
    Update-Database

  - dotnet CLI (EF Core):
    dotnet ef database update

Example table layout (typical)

- Students (StudentId PK, FirstName, LastName, DateOfBirth, Email)
- Courses (CourseId PK, Title, Credits, Description)
- Enrollments (EnrollmentId PK, StudentId FK, CourseId FK, Grade)
- Staff/Teachers (StaffId PK, FirstName, LastName, Role)

If you don't have the SQL scripts in the repository, create the schema by adapting the example above.

## Configuration

Connection strings are commonly stored in one of these locations depending on project type:

- app.config / web.config (classic .NET Framework)
- appsettings.json (ASP.NET Core / .NET Core projects)
- A dedicated settings file or environment variables

Example connection strings:

SQL Server (LocalDB):

```
Server=(localdb)\mssqllocaldb;Database=SchoolDb;Trusted_Connection=True;
```

SQL Server (named instance):

```
Server=localhost\SQLEXPRESS;Database=SchoolDb;Trusted_Connection=True;User Id=sa;Password=your_password;
```

SQLite (file-based):

```
Data Source=school.db
```

How to change the connection string:

- Open `app.config`/`web.config` or `appsettings.json` and update the connection string entry.
- If the repository reads connection details from code, search for a variable named `connectionString` or `ConnectionString` and update it.

## Build and run

1. Clone the repository:

   git clone https://github.com/liewvk/Csharp-SchoolManagementSystem.git
   cd Csharp-SchoolManagementSystem

2. Open the solution in Visual Studio or use the dotnet CLI:

   - Visual Studio: open the `.sln` file and build the solution (Build > Build Solution)
   - dotnet CLI: `dotnet build` (only for SDK-style projects)

3. Ensure you have created the database and updated the connection string as described above.

4. Run the application from Visual Studio (F5) or with the dotnet CLI (`dotnet run` from the project directory) if supported.

If the project is a desktop WinForms/WPF application, start it from Visual Studio. If it is a console app, run the produced executable or `dotnet run`.

## Project structure (typical)

- /src or / (root): C# project files (.csproj), solution file (.sln)
- /Models: domain models (Student, Course, Enrollment, Staff)
- /Data or /DAL: data access layer, repository pattern or EF DbContext
- /Services: business logic
- /UI: console/WinForms/WPF/ASP.NET UI code
- /sql or /scripts: SQL schema and seed data
- /tests: unit/integration tests

Adjust the above to match this repository's actual folders.

## Usage examples

- Add a student: run the app and choose the menu option to create a student, or call the AddStudent method from code
- List students: use the query UI or run a provided sample script
- Enroll a student: assign a student to a course through the UI or by calling the enrollment method

If the repository includes example command-line options or sample code snippets, consult those files for exact usage.

## Tests

If the repository contains a `tests` or `UnitTests` project, run tests using Visual Studio Test Explorer or the dotnet CLI:

```
dotnet test
```

If there are no tests, consider adding unit tests around the data access and business logic.

## Contributing

Contributions are welcome. Suggested steps:

1. Fork the repository
2. Create a branch for your feature/fix: `git checkout -b feature/your-feature`
3. Make changes and add tests if appropriate
4. Commit and push: `git commit -am "Add feature"` then `git push origin feature/your-feature`
5. Open a pull request describing your changes

Please follow consistent coding styles and include clear commit messages.

## Troubleshooting

- Database connection errors: check connection string, make sure the DB server is running, and verify firewall/permissions
- Schema mismatch: run schema scripts or migrations
- Missing NuGet packages: restore packages in Visual Studio or run `dotnet restore`

If you encounter runtime exceptions, look at the inner exception for database errors (SqlException) and check the stack trace.

## License

If the original project provided a license, keep using that. If no license is present, add one if you intend to open-source the project. A common permissive choice is the MIT license.

## Author / Contact

Author: liewvk

For questions about the sample code, open an issue in this repository and provide details and reproduction steps.

---

If you'd like, I can:

- Tailor this README specifically to the exact project files (detect target .NET runtime, database, and include precise build/run steps) by inspecting the repository contents and adding example connection strings and commands. Would you like me to do that now?