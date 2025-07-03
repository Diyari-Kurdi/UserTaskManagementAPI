# UserTaskManagementAPI

A lightweight ASP.NET Core 9.0 Web API built with Minimal APIs, supporting user registration, login, and task management with secure JWT authentication, role-based access control, and advanced filtering and pagination features.

---

## Features

-  JWT-based Authentication & Role-based Authorization
-  User Management (Admin & User roles)
-  CRUD operations for Tasks
-  Task Priority support (Low, Medium, High)
-  Tagging, Search, Pagination, and Filtering
-  Swagger UI for testing
-  PostgreSQL + Entity Framework Core

---

## Core Entities

### User

- `Id` (GUID)
- `Username` (string)
- `PasswordHash` (string)
- `Role` (string): `"Admin"` or `"User"`

### TaskItem

- `Id` (GUID)
- `Title` (string)
- `Description` (string)
- `IsCompleted` (bool)
- `Priority` (enum): `"Low"`, `"Medium"`, `"High"` (default: `Low`)
- `CreatedByUserId` (GUID)
- `Tags` (string[])

---

## Authentication & Authorization

- JWT-based login
- Role-based authorization using built-in ASP.NET Core policies
- Only authenticated users can access tasks
- Users manage their own tasks
- Admins can manage all tasks

---

## API Endpoints

| Method | Route                | Description                                | Access        |
|--------|---------------------|--------------------------------------------|---------------|
| POST   | `/api/auth/register`| Register new user                          | Public        |
| POST   | `/api/auth/login`   | Authenticate and return JWT token          | Public        |
| GET    | `/api/tasks`        | Get tasks (own for User, all for Admin)    | Authenticated |
| POST   | `/api/tasks`        | Create a new task                          | Authenticated |
| PUT    | `/api/tasks/{id}`   | Update a task (Owner/Admin)                | Owner/Admin   |
| DELETE | `/api/tasks/{id}`   | Delete a task (Owner/Admin)                | Owner/Admin   |

---

## Filtering & Pagination

The `GET /api/tasks` endpoint supports both filtering and pagination. All parameters are **optional**, but:

> ⚠️ `pageNumber` and `pageSize` are **optional**, but if you provide one, you **must provide both**.

> Supplying only one will result in a **400 Bad Request**.

| Parameter        | Type            | Description                                                           |
| ---------------- | --------------- | --------------------------------------------------------------------- |
| `pageNumber`     | `int?`          | Page number for pagination                                            |
| `pageSize`       | `int?`          | Page size for pagination                                              |
| `isCompleted`    | `bool?`         | Filter tasks by completion status (`true` or `false`)                 |
| `tag`            | `string?`       | Filter by a specific tag                                              |
| `search`         | `string?`       | Keyword search in the task's title or description                     |
| `priority`       | `TaskPriority?` | Filter by task priority (`Low`, `Medium`, `High`)                     |
| `sortByPriority` | `SortType?`     | Sort by priority (`Ascending` or `Descending`); default: by task `Id` |

> * If **no parameters** are provided, all tasks will be returned (admin: all users’ tasks, user: own tasks).
> * If **only filters** are provided (e.g., `tag`, `priority`, etc.), filtered results will be returned.
> * If **filters + pagination** are provided, results will be both filtered and paginated.

### Example: Filter + Pagination

```http
GET /api/tasks?pageNumber=1&pageSize=10&isCompleted=false&tag=work&priority=High&search=report&sortByPriority=Descending
```

---

## Setup & Run Instructions

1. **Clone the repo**
   ```bash
   git clone https://github.com/Diyari-Kurdi/UserTaskManagementAPI.git
   cd UserTaskManagementAPI
   ```

2. **Configure PostgreSQL connection**
   - See the [configuration](https://github.com/Diyari-Kurdi/UserTaskManagementAPI?tab=readme-ov-file#configuration) section below.

3. **Install EF Core CLI tool (if not already installed)**
   ```bash
   dotnet tool install --global dotnet-ef
   ```

4. **Run EF Core migrations**
   ```bash
   dotnet ef database update
   ```

5. **Run the API**
   ```bash
   dotnet run
   ```

6. **Open Swagger**
   - Visit `https://localhost:7217/swagger`

---

## Configuration

Before running the application, update your `appsettings.json` or `appsettings.Development.json` file with the appropriate values:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=TaskDb;Username=postgres;Password=your_password"
  },
  "JwtSettings": {
    "Secret": "your_jwt_secret_here",
    "Issuer": "UserTaskApi",
    "Audience": "UserTaskApiAudience",
    "ExpiresInMinutes": 60
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

---

## Design Decisions & Assumptions

- Uses Minimal APIs for simplicity and performance.
- Secure password hashing implemented.
- JWT tokens include user role claims.
- Swagger configured to support enum values.

---

## API Testing

Test via:
- Swagger: `https://localhost:7217/swagger`

---
