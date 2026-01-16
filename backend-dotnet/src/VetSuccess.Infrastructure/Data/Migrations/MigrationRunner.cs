using Microsoft.Extensions.Logging;
using Npgsql;

namespace VetSuccess.Infrastructure.Data.Migrations;

/// <summary>
/// Utility class for running SQL migrations programmatically
/// </summary>
public class MigrationRunner
{
    private readonly string _connectionString;
    private readonly ILogger<MigrationRunner> _logger;

    public MigrationRunner(string connectionString, ILogger<MigrationRunner> logger)
    {
        _connectionString = connectionString;
        _logger = logger;
    }

    /// <summary>
    /// Applies all pending migrations
    /// </summary>
    public async Task ApplyAllMigrationsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting migration process");

        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            // Read the master migration script
            var scriptPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Data",
                "Migrations",
                "apply_all_migrations.sql"
            );

            if (!File.Exists(scriptPath))
            {
                _logger.LogWarning("Migration script not found at {ScriptPath}", scriptPath);
                return;
            }

            var script = await File.ReadAllTextAsync(scriptPath, cancellationToken);

            await using var command = new NpgsqlCommand(script, connection);
            command.CommandTimeout = 300; // 5 minutes timeout

            await command.ExecuteNonQueryAsync(cancellationToken);

            _logger.LogInformation("All migrations applied successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error applying migrations");
            throw;
        }
    }

    /// <summary>
    /// Rolls back all migrations
    /// </summary>
    public async Task RollbackAllMigrationsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogWarning("Starting migration rollback process");

        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            // Read the rollback script
            var scriptPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Data",
                "Migrations",
                "rollback_all_migrations.sql"
            );

            if (!File.Exists(scriptPath))
            {
                _logger.LogWarning("Rollback script not found at {ScriptPath}", scriptPath);
                return;
            }

            var script = await File.ReadAllTextAsync(scriptPath, cancellationToken);

            await using var command = new NpgsqlCommand(script, connection);
            command.CommandTimeout = 300; // 5 minutes timeout

            await command.ExecuteNonQueryAsync(cancellationToken);

            _logger.LogInformation("All migrations rolled back successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rolling back migrations");
            throw;
        }
    }

    /// <summary>
    /// Gets the list of applied migrations
    /// </summary>
    public async Task<List<MigrationInfo>> GetAppliedMigrationsAsync(CancellationToken cancellationToken = default)
    {
        var migrations = new List<MigrationInfo>();

        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            // Check if migrations table exists
            var checkTableSql = @"
                SELECT EXISTS (
                    SELECT FROM information_schema.tables 
                    WHERE table_name = '__migrations'
                )";

            await using var checkCommand = new NpgsqlCommand(checkTableSql, connection);
            var tableExists = (bool)(await checkCommand.ExecuteScalarAsync(cancellationToken) ?? false);

            if (!tableExists)
            {
                _logger.LogInformation("Migrations table does not exist yet");
                return migrations;
            }

            // Get applied migrations
            var sql = @"
                SELECT migration_name, applied_at, applied_by 
                FROM __migrations 
                ORDER BY id";

            await using var command = new NpgsqlCommand(sql, connection);
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                migrations.Add(new MigrationInfo
                {
                    Name = reader.GetString(0),
                    AppliedAt = reader.GetDateTime(1),
                    AppliedBy = reader.GetString(2)
                });
            }

            _logger.LogInformation("Found {Count} applied migrations", migrations.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving applied migrations");
            throw;
        }

        return migrations;
    }
}

/// <summary>
/// Information about an applied migration
/// </summary>
public class MigrationInfo
{
    public string Name { get; set; } = string.Empty;
    public DateTime AppliedAt { get; set; }
    public string AppliedBy { get; set; } = string.Empty;
}
