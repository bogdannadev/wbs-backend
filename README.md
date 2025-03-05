# BonusSystem Prototype

A bonus tracking and management platform designed to provide a flexible, scalable solution for managing bonus transactions across multiple user groups.

## Project Overview

BonusSystem is a comprehensive platform that allows different user roles (buyers, sellers, admins, etc.) to interact with a bonus point system. It follows a Vertical Slice Monolith with BFF (Backend for Frontend) architecture using .NET 9 and Supabase as the backend service.

## Architecture

- **Architecture Style**: Monolithic with Modular Design
- **Technology Stack**:
  - Backend: .NET 9, PostgreSQL (via Entity Framework Core)
  - Authentication: JWT-based Authentication

## Project Structure

- **BonusSystem.Api**: Web API with BFF implementation
- **BonusSystem.Core**: Domain models and business logic
- **BonusSystem.Infrastructure**: Database integration and external services
- **BonusSystem.Shared**: DTOs and shared utilities

## Database Configuration

The system supports two database options:

1. **In-Memory Database**: 
   - For development and testing
   - No external dependencies required

2. **PostgreSQL Database**:
   - For production use
   - Requires PostgreSQL 14+ installed
   - Entity Framework Core migrations for schema management

To configure the database, edit the `appsettings.json`:

```json
{
  "AppDb": {
    "Type": "Postgres", // Options: "InMemory" or "Postgres"
    ...
  },
  "Postgres": {
    "ConnectionString": "Host=localhost;Database=bonussystem;Username=postgres;Password=postgres",
    "EnableDetailedLogging": true,
    "ApplyMigrationsAtStartup": true
  }
}
```

## Setting Up PostgreSQL

1. Install PostgreSQL 14+ on your system
2. Create a database named `bonussystem` (or as specified in your connection string)
3. Run database migrations:

```bash
# From the solution root
cd src/BonusSystem.Infrastructure
dotnet ef migrations add InitialCreate --startup-project ../BonusSystem.Api --output-dir DataAccess/Postgres/Migrations
dotnet ef database update --startup-project ../BonusSystem.Api
```

Alternatively, set `ApplyMigrationsAtStartup` to `true` in the configuration to have the application apply migrations automatically.

## Testing Framework

The project includes a comprehensive testing framework structured as follows:

- **BonusSystem.Core.Tests**: Unit tests for core domain services and business logic
- **BonusSystem.Infrastructure.Tests**: Tests for infrastructure components and database integration
- **BonusSystem.Api.Tests**: Integration tests for API endpoints

### Testing Libraries

- xUnit: Testing framework
- Moq: Mocking framework for isolating components during testing
- FluentAssertions: For more readable assertions
- Microsoft.AspNetCore.Mvc.Testing: For API integration tests

### Running Tests

To run the tests, use the following command:

```bash
dotnet test tests/BonusSystem.Tests.sln
```

or run individual test projects:

```bash
dotnet test tests/BonusSystem.Core.Tests
```
