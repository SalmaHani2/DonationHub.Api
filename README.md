
# DonationHub API

DonationHub is a donation management system built with ASP.NET Core Web API and .NET 10. It allows users to browse donation campaigns, make donations, and manage campaigns based on their assigned roles.

## 1. Prerequisites

Make sure you have the following installed:

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- SQL Server (SQL Server Express, Developer Edition, or LocalDB)
- Visual Studio 2022 or Visual Studio Code
- Entity Framework Core CLI

Install the EF Core CLI tool if it is not already installed:

```bash
dotnet tool install --global dotnet-ef
```

Or update it:

```bash
dotnet tool update --global dotnet-ef
```

Verify your installation:

```bash
dotnet --version
dotnet ef --version
```

## 2. Project Structure

The solution follows Clean Architecture with clear separation of responsibilities.

```text
DonationHub/
│
├── DonationHub.Domain/
│   ├── Entities/
│   ├── Enums/
│   └── ValueObjects/
│
├── DonationHub.Application/
│   ├── Interfaces/
│   ├── Services/
│   └── Options/
│
├── DonationHub.Infrastructure/
│   ├── Persistence/
│   ├── Repositories/
│   ├── Identity/
│   └── Services/
│
├── DonationHub.ApiDonationHub.Api/
│   ├── Controllers/
│   ├── Extensions/
│   ├── Program.cs
│   └── appsettings.json
│
└── DonationHub.sln
```

## 3. Configuration

The application uses .NET User Secrets to store sensitive development configuration.

Never commit real passwords, JWT secrets, or other credentials to source control.

### 3.1 Configure User Secrets

Navigate to the API project directory (the directory containing the API `.csproj` file):

```bash
cd DonationHub.ApiDonationHub.Api
```

Initialize User Secrets if they have not already been initialized:

```bash
dotnet user-secrets init
```

### 3.2 Database Connection String

Configure your SQL Server connection string:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=YOUR_SERVER;Database=DonationHubDb;Trusted_Connection=True;TrustServerCertificate=True;"
```

Replace `YOUR_SERVER` with your SQL Server instance.

For example, for LocalDB:

```text
Server=(localdb)\MSSQLLocalDB;Database=DonationHubDb;Trusted_Connection=True;TrustServerCertificate=True;
```

### 3.3 Seeded Admin Credentials

Configure the credentials used by the database seeder to create the initial Admin account:

```bash
dotnet user-secrets set "SeedAdmin:Email" "YOUR_ADMIN_EMAIL"
dotnet user-secrets set "SeedAdmin:Password" "YOUR_STRONG_ADMIN_PASSWORD"
```

Use your own development credentials. Never commit actual credentials.

### 3.4 Development Test User Credentials

If the application requires a development test user, configure its credentials using the corresponding configuration keys:

```bash
dotnet user-secrets set "TestUser:Email" "YOUR_TEST_USER_EMAIL"
dotnet user-secrets set "TestUser:Password" "YOUR_TEST_USER_PASSWORD"
```

Make sure these keys match the configuration section used by your development seeder.

### 3.5 JWT Configuration

Configure JWT settings:

```bash
dotnet user-secrets set "Jwt:SecretKey" "YOUR_SECRET_KEY_AT_LEAST_32_BYTES"
dotnet user-secrets set "Jwt:Issuer" "DonationHub"
dotnet user-secrets set "Jwt:Audience" "DonationHub"
```

The JWT secret key must contain at least 32 bytes.

### 3.6 Verify User Secrets

List the configured keys:

```bash
dotnet user-secrets list
```

User Secrets are stored outside the project directory and should never be committed to Git.

## 4. Database Setup

The project uses Entity Framework Core Code-First migrations.

Make sure SQL Server is running and the connection string is correctly configured before applying migrations.

### 4.1 Apply Migrations

From the solution directory, run:

```bash
dotnet ef database update --project DonationHub.Infrastructure --startup-project DonationHub.ApiDonationHub.Api
```

This command:

- Uses the Infrastructure project to locate the migrations and DbContext.
- Uses the API project to load application configuration and dependency injection.
- Creates the database if it does not exist.
- Applies all pending migrations.

### 4.2 Create a New Migration

When database models or configurations change, create a migration:

```bash
dotnet ef migrations add MigrationName --project DonationHub.Infrastructure --startup-project DonationHub.ApiDonationHub.Api
```

Then apply it:

```bash
dotnet ef database update --project DonationHub.Infrastructure --startup-project DonationHub.ApiDonationHub.Api
```

## 5. Running the Application

From the API project directory:

```bash
dotnet run
```

Or run the API using Visual Studio.

The application will start on the configured HTTP/HTTPS URLs.

Check the console output to identify the actual listening address.

## 6. API Documentation

DonationHub uses OpenAPI for API documentation and Scalar as its interactive API documentation interface.

After starting the application, access:

- **Scalar UI:** `http://localhost:5000/scalar`
- **OpenAPI JSON:** `http://localhost:5000/openapi/v1.json`

The actual URLs depend on the configured application ports.

### Testing Protected Endpoints

Some endpoints require authentication and specific roles.

To test protected endpoints:

1. Open Scalar.
2. Execute the login endpoint using valid credentials.
3. Copy the returned JWT access token.
4. Click the authentication/Authorize option in Scalar.
5. Select Bearer authentication and enter your access token.
6. Execute the protected endpoint.

Make sure the authenticated user has the required role.

For example:

- Admin-only endpoints require the `Admin` role.
- Staff management endpoints may require `Admin` or `CampaignManager`.
- Anonymous endpoints can be accessed without authentication.

## 7. Design Decisions

### 7.1 Clean Architecture

The application follows Clean Architecture to separate business rules from infrastructure and presentation concerns.

The main layers are:

- **Domain:** Contains entities, enums, and value objects representing the core business concepts.
- **Application:** Contains application interfaces, service contracts, and use-case logic.
- **Infrastructure:** Implements persistence, repositories, Identity, and external infrastructure concerns.
- **API:** Exposes HTTP endpoints through controllers and handles request/response communication.

Dependencies point inward, keeping the Domain independent of infrastructure details.

### 7.2 Generic Repository + Unit of Work for Writes

Write operations go through the Generic Repository and Unit of Work abstractions.

This provides:

- A centralized approach to data modification.
- Separation between application logic and EF Core persistence details.
- A consistent way to commit changes.
- Better control over transaction boundaries and persistence operations.

The Unit of Work coordinates changes and commits them through a single `SaveChangesAsync` operation.

### 7.3 Direct DbContext Access for Reads

Read operations use the EF Core `DbContext` directly with:

- `AsNoTracking()` to avoid unnecessary change tracking.
- DTO projection using `Select()` to retrieve only the required fields.

This follows **SRS §3.3, Rule 5**, which specifies separating read and write access patterns.

Benefits include:

- Reduced memory overhead.
- Better query performance.
- Fetching only the required data.
- Avoiding unnecessary entity tracking for read-only operations.

This approach keeps write operations structured through Repository + Unit of Work while allowing optimized read queries.

### 7.4 Authentication and Authorization

The application uses ASP.NET Core Identity for user management and JWT Bearer authentication for securing API endpoints.

Role-based authorization controls access to protected functionality.

### 7.5 Soft Delete

Entities that support soft deletion are marked as deleted instead of being physically removed from the database.

Global query filters exclude soft-deleted records from normal queries.

### 7.6 API Error Handling

The API uses standardized error responses through ProblemDetails to provide consistent and meaningful HTTP error information.

---

## 8. Security Notes

- Never commit real credentials or JWT secrets.
- Use User Secrets for local development.
- Use environment variables or a secure secret manager in production.
- Do not expose sensitive configuration values in logs or API responses.
- Use HTTPS in production.
- Keep authentication and authorization enabled for protected endpoints.

## 9. Technologies Used

- C#
- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- ASP.NET Core Identity
- JWT Bearer Authentication
- Scalar
- OpenAPI
- Clean Architecture
- Generic Repository
- Unit of Work
