# DonationHub

DonationHub is a donation management API built with **ASP.NET Core .NET 10**, following Clean Architecture principles.

## Prerequisites

Before running the project, make sure you have:

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- SQL Server (LocalDB, SQL Server Express, or a full SQL Server instance)
- An IDE or editor such as Visual Studio or Visual Studio Code
- Entity Framework Core CLI tools

Install the EF Core CLI tool if you haven't already:

```bash
dotnet tool install --global dotnet-ef
```

If it is already installed, update it:

```bash
dotnet tool update --global dotnet-ef
```

## Getting Started

### 1. Clone the repository

```bash
git clone <repository-url>
cd DonationHub
```

### 2. Configure User Secrets

The project uses ASP.NET Core User Secrets to store sensitive development settings.

Navigate to the API project directory or specify its project file when running the commands.

Initialize User Secrets:

```bash
dotnet user-secrets init --project <path-to-api-csproj>
```

Set the database connection string:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<your-sql-server-connection-string>" --project <path-to-api-csproj>
```

Set the JWT configuration:

```bash
dotnet user-secrets set "Jwt:SecretKey" "<your-secret-key>" --project <path-to-api-csproj>
```

Set the seeded Admin credentials:

```bash
dotnet user-secrets set "SeedAdmin:Email" "<admin-email>" --project <path-to-api-csproj>
dotnet user-secrets set "SeedAdmin:Password" "<admin-password>" --project <path-to-api-csproj>
```

Set development test-user credentials if the development seeder supports them:

```bash
dotnet user-secrets set "DevTestUser:Email" "<test-user-email>" --project <path-to-api-csproj>
dotnet user-secrets set "DevTestUser:Password" "<test-user-password>" --project <path-to-api-csproj>
```

> **Important:** Never commit real passwords, JWT secret keys, or database credentials to source control. The configuration keys above must match the sections and property names used by the project's Options classes and seeders.

### 3. Apply Database Migrations

From the repository root, run:

```bash
dotnet ef database update --project <path-to-infrastructure-csproj> --startup-project <path-to-api-csproj> --context AppDbContext
```

This applies the existing EF Core migrations and creates or updates the database schema.

If the project does not have an initial migration yet, create one first:

```bash
dotnet ef migrations add InitialCreate --project <path-to-infrastructure-csproj> --startup-project <path-to-api-csproj> --context AppDbContext
```

### 4. Run the API

```bash
dotnet run --project <path-to-api-csproj>
```

Check the terminal output for the application URL.

## API Documentation

When running in the Development environment, the API documentation can be accessed through **Scalar**:

```text
http://localhost:<port>/scalar
```

The OpenAPI document is available at:

```text
http://localhost:<port>/openapi/v1.json
```

Replace `<port>` with the port shown in the application startup output.

If Swagger UI is configured separately, use its configured URL. Otherwise, Scalar is the interactive API documentation interface.

## Testing Protected Endpoints

Some endpoints require authentication.

1. Open Scalar and locate `POST /auth/login`.
2. Log in using a valid user account.
3. Copy the returned JWT Access Token.
4. For protected endpoints, provide the token using the Bearer authentication scheme:

```text
Authorization: Bearer <your-access-token>
```

5. Execute the protected endpoint and inspect its response.

For endpoints requiring specific roles, make sure the logged-in user has the required role.

**Authentication endpoints to test:**

- `POST /auth/register` — Register a new donor.
- `POST /auth/login` — Authenticate and receive tokens.
- `POST /auth/refresh` — Refresh the access token using the refresh token.
- `POST /auth/logout` — Revoke the provided refresh token.
- `GET /auth/me` — Retrieve the authenticated user's information, if implemented.

## Design Decisions

### Clean Architecture

The solution separates responsibilities into layers:

- **Domain:** Contains core entities, value objects, and business rules.
- **Application:** Contains application interfaces, contracts, and use cases.
- **Infrastructure:** Contains persistence, EF Core configurations, and external service implementations.
- **API:** Exposes HTTP endpoints and handles incoming requests.

This separation keeps business logic independent of infrastructure and presentation concerns.

### Generic Repository and Unit of Work for Writes

Following **SRS §3.3, Rule 5**, write operations go through the Generic Repository and Unit of Work abstractions.

- The Generic Repository provides a common way to add, update, and remove entities.
- The Unit of Work coordinates persistence and commits changes through `SaveChangesAsync`.
- This keeps write operations consistent and separates application-level operations from EF Core persistence details.

### Direct DbContext Queries for Reads

Read operations use `DbContext` directly with:

- `AsNoTracking()` to avoid unnecessary change tracking for read-only queries.
- DTO projection using `Select()` to retrieve only the required fields.

This avoids loading and tracking complete entities when the application only needs to return response data.

It also supports efficient read queries and avoids unnecessary repository abstractions for simple query operations.

## Security Notes

- Never store plaintext passwords. ASP.NET Core Identity handles password hashing.
- Store hashed refresh tokens in the database rather than their original values.
- Keep JWT signing keys and other sensitive configuration in User Secrets during development.
- Do not commit real credentials or secrets to the repository.
