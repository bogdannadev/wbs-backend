namespace BonusSystem.Infrastructure.DataAccess.Postgres;

public class PostgresOptions
{
    public const string Position = "Postgres";

    public string ConnectionString { get; set; } = string.Empty;
    public bool EnableDetailedLogging { get; set; } = false;
    public bool ApplyMigrationsAtStartup { get; set; } = true;
}