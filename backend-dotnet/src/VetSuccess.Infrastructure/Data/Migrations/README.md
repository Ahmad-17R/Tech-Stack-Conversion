# Database Migrations

This folder contains SQL migration scripts for the VetSuccess database.

## Migration Files

### 001_AddSmsAndEmailEventTables.sql

Creates three new tables:

- `sms_events` - Stores scheduled SMS sending events
- `updates_email_events` - Stores daily email update events for practices
- `sms_templates` - Stores SMS templates with keyword matching

**Rollback:** `001_AddSmsAndEmailEventTables_Rollback.sql`

## Running Migrations

### Quick Start (Recommended)

Use the master migration script to apply all migrations at once:

```bash
# Apply all migrations
psql -h <host> -U <username> -d <database> -f apply_all_migrations.sql

# Rollback all migrations
psql -h <host> -U <username> -d <database> -f rollback_all_migrations.sql
```

The master scripts include:

- Migration tracking table (`__migrations`)
- Automatic detection of already-applied migrations
- Safe re-run capability (idempotent)
- Detailed logging of migration status

### Option 1: Using psql (PostgreSQL Command Line)

```bash
# Apply migration
psql -h <host> -U <username> -d <database> -f 001_AddSmsAndEmailEventTables.sql

# Rollback migration
psql -h <host> -U <username> -d <database> -f 001_AddSmsAndEmailEventTables_Rollback.sql
```

### Option 2: Using pgAdmin or DBeaver

1. Connect to your database
2. Open the SQL script file
3. Execute the script

### Option 3: Using EF Core Migrations (Recommended for Development)

The application is configured to automatically apply migrations in development mode. See `Program.cs`:

```csharp
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<VetSuccessDbContext>();
    await dbContext.Database.MigrateAsync();
}
```

To generate EF Core migrations from the entity configurations:

```bash
# Install EF Core tools (if not already installed)
dotnet tool install --global dotnet-ef

# Generate migration
dotnet ef migrations add <MigrationName> \
  --project src/VetSuccess.Infrastructure/VetSuccess.Infrastructure.csproj \
  --startup-project src/VetSuccess.Api/VetSuccess.Api.csproj \
  --context VetSuccessDbContext

# Apply migrations
dotnet ef database update \
  --project src/VetSuccess.Infrastructure/VetSuccess.Infrastructure.csproj \
  --startup-project src/VetSuccess.Api/VetSuccess.Api.csproj \
  --context VetSuccessDbContext
```

## Migration Naming Convention

Migrations follow the pattern: `<number>_<DescriptiveName>.sql`

- Number: Sequential 3-digit number (001, 002, etc.)
- DescriptiveName: PascalCase description of the migration
- Rollback files: Same name with `_Rollback` suffix

## Production Deployment

For production deployments:

1. **Review the migration script** - Ensure it's safe for production
2. **Backup the database** - Always backup before applying migrations
3. **Test in staging** - Apply and test in a staging environment first
4. **Apply during maintenance window** - Schedule migrations during low-traffic periods
5. **Monitor the application** - Watch for errors after deployment

### Production Migration Checklist

- [ ] Database backup completed
- [ ] Migration tested in staging environment
- [ ] Rollback script tested
- [ ] Maintenance window scheduled
- [ ] Team notified of deployment
- [ ] Monitoring alerts configured
- [ ] Migration applied successfully
- [ ] Application health verified
- [ ] Rollback plan ready if needed

## Notes

- All timestamps use `TIMESTAMP WITH TIME ZONE` for proper timezone handling
- UUIDs are used as primary keys for distributed system compatibility
- JSONB columns are used for flexible schema storage (context, file_paths)
- Foreign key constraints use `ON DELETE RESTRICT` to prevent accidental data loss
- Indexes are created for commonly queried columns to optimize performance
- Comments are added to tables and columns for documentation

## Troubleshooting

### Migration fails with "relation already exists"

The migration uses `IF NOT EXISTS` clauses, so it's safe to re-run. If you still encounter issues, check if the tables were partially created and manually clean up before re-running.

### Foreign key constraint violation

Ensure the `practices` table exists and has the `practice_odu_id` column before running this migration.

### Permission denied

Ensure your database user has CREATE TABLE, CREATE INDEX, and COMMENT privileges.
