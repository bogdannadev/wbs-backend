# Database Migrations

Migrations are generated using Entity Framework Core tools.

## Generate Initial Migration

```bash
# From the solution root
cd src/BonusSystem.Infrastructure
dotnet ef migrations add InitialCreate --startup-project ../BonusSystem.Api --output-dir DataAccess/Postgres/Migrations
```

## Apply Migrations Manually

```bash
# From the solution root
cd src/BonusSystem.Infrastructure
dotnet ef database update --startup-project ../BonusSystem.Api
```

Note: If `ApplyMigrationsAtStartup` is set to `true` in PostgresOptions (in appsettings.json), 
migrations will be automatically applied when the application starts.
