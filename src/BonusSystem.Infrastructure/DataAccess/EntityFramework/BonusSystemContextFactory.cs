using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace BonusSystem.Infrastructure.DataAccess.EntityFramework;

// This class is used by EF Core tools at design time to create migrations
public class BonusSystemContextFactory : IDesignTimeDbContextFactory<BonusSystemContext>
{
    public BonusSystemContext CreateDbContext(string[] args)
    {
        // Get environment name with fallback to Development
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        
        // Get current directory for configuration path
        var basePath = Directory.GetCurrentDirectory();
        
        // Navigate to API project to find appsettings
        var apiProjectPath = Path.GetFullPath(Path.Combine(basePath, "../../BonusSystem.Api"));
        
        // Create configuration
        var builder = new ConfigurationBuilder();
        
        // Build the configuration path instead of using SetBasePath
        string jsonPath = Path.Combine(apiProjectPath, "appsettings.json");
        builder.AddJsonFile(jsonPath, optional: false, reloadOnChange: true);
        
        string envJsonPath = Path.Combine(apiProjectPath, $"appsettings.{environmentName}.json");
        builder.AddJsonFile(envJsonPath, optional: true, reloadOnChange: true);
        
        builder.AddEnvironmentVariables();
        
        var configuration = builder.Build();
        
        // Get connection string
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Could not find a connection string named 'DefaultConnection'.");
        }
        
        // Configure and create the context
        var optionsBuilder = new DbContextOptionsBuilder<BonusSystemContext>();
        optionsBuilder.UseNpgsql(connectionString, npgOptions =>
        {
            npgOptions.MigrationsHistoryTable("__ef_migrations_history", "bonus");
            npgOptions.EnableRetryOnFailure(5);
        });
        
        return new BonusSystemContext(optionsBuilder.Options);
    }
}