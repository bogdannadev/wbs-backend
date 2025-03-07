# PostgreSQL Implementation for BonusSystem

This document outlines the PostgreSQL implementation for the BonusSystem prototype.

## Overview

The BonusSystem prototype uses a PostgreSQL database for permanent storage, configured with Entity Framework Core as the ORM. The implementation follows the repository pattern, with clear separation between the domain interfaces and the data access implementations.

## Components

### Database Context

The `BonusSystemDbContext` class is the central component that manages the database connection and entity mappings. It configures:

- Entity relationships and navigation properties
- Table and column naming
- Unique constraints and indexes
- Schema organization (using a "bonus" schema)

### Repositories

The following repositories have been implemented:

1. **PostgresUserRepository** - Manages user data storage and retrieval
2. **PostgresCompanyRepository** - Manages company data and balances
3. **PostgresStoreRepository** - Manages store listings and relationships
4. **PostgresTransactionRepository** - Handles bonus transactions with audit trail
5. **PostgresNotificationRepository** - Manages notifications to users

All repositories follow a consistent pattern:
- Mapping between entity and DTO models
- Error handling and logging
- Asynchronous operations
- CRUD operations implementation

### Entities

The entity model includes the following main types:

- **UserEntity** - Represents system users with various roles
- **CompanyEntity** - Represents companies that participate in the bonus system
- **StoreEntity** - Physical stores belonging to companies
- **TransactionEntity** - Records of bonus transactions
- **NotificationEntity** - System notifications
- **StoreSellerEntity** - Mapping between sellers and their assigned stores

### Migrations

Initial database schema migration has been set up with:

- Tables for all core entities
- Relationships and foreign keys
- Indexes for performance
- Constraints for data integrity

## Configuration

The PostgreSQL implementation can be configured through the `PostgresOptions` settings, which include:

- **ConnectionString** - Database connection information
- **EnableDetailedLogging** - Toggle for verbose logging of SQL operations
- **ApplyMigrationsAtStartup** - Automatically updates database schema on startup

## Setup Instructions

1. Ensure PostgreSQL is running and accessible
2. Configure connection string in your environment settings
3. Set `AppDb:Type` to "postgres" in configuration

## Usage

The `PostgresDataService` class registers all repositories and makes them available through the `IDataService` interface. Client code can access repositories through this interface without being aware of the underlying PostgreSQL implementation.

```csharp
// Example usage
public class SomeService
{
    private readonly IUserRepository _userRepository;
    
    public SomeService(IDataService dataService)
    {
        _userRepository = dataService.Users;
    }
    
    public async Task<UserDto?> GetUserAsync(Guid userId)
    {
        return await _userRepository.GetByIdAsync(userId);
    }
}
```

## Benefits of PostgreSQL

1. **Robust Transactional Support** - Ensures data integrity during complex operations
2. **Advanced Querying** - Supports complex queries and data aggregation
3. **Scalability** - Can handle large data volumes
4. **Data Integrity** - Enforces relationships and constraints
5. **JSON Support** - Useful for semi-structured data
6. **Rich Indexing Options** - Improves query performance

## Future Improvements

1. Implement query caching for better performance
2. Add paging for large result sets
3. Implement soft delete functionality
4. Add full-text search capabilities
5. Implement batch operations for bulk processing
