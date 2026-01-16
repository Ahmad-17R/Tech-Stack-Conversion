#!/bin/bash
# VetSuccess Database Setup Script
# Bash script to automate database setup on Linux/Mac

SERVER_NAME="${1:-localhost}"
DATABASE_NAME="${2:-VetSuccessDb}"
USERNAME="${3:-sa}"
PASSWORD="${4}"
DROP_EXISTING="${5:-false}"

echo "========================================"
echo "VetSuccess Database Setup"
echo "========================================"
echo ""

# Check if sqlcmd is available
if ! command -v sqlcmd &> /dev/null; then
    echo "✗ sqlcmd not found. Please install SQL Server Command Line Utilities"
    echo "Download from: https://docs.microsoft.com/en-us/sql/linux/sql-server-linux-setup-tools"
    exit 1
fi

echo "✓ sqlcmd found"
echo "Server: $SERVER_NAME"
echo "Database: $DATABASE_NAME"
echo ""

# Check password
if [ -z "$PASSWORD" ]; then
    echo "Error: Password required"
    echo "Usage: ./setup-database.sh [server] [database] [username] [password] [drop_existing]"
    echo "Example: ./setup-database.sh localhost VetSuccessDb sa MyPassword123"
    exit 1
fi

# Drop existing database if requested
if [ "$DROP_EXISTING" = "true" ]; then
    echo "Dropping existing database..."
    sqlcmd -S $SERVER_NAME -U $USERNAME -P $PASSWORD -Q "
        USE master;
        IF EXISTS (SELECT name FROM sys.databases WHERE name = '$DATABASE_NAME')
        BEGIN
            ALTER DATABASE [$DATABASE_NAME] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
            DROP DATABASE [$DATABASE_NAME];
            PRINT 'Database dropped successfully';
        END
    "
    echo "✓ Existing database dropped"
    echo ""
fi

# Run setup scripts
echo "Step 1: Creating database and core tables..."
sqlcmd -S $SERVER_NAME -U $USERNAME -P $PASSWORD -i database-complete-schema.sql

if [ $? -ne 0 ]; then
    echo "✗ Error creating core tables"
    exit 1
fi
echo "✓ Core tables created"
echo ""

echo "Step 2: Creating additional tables..."
sqlcmd -S $SERVER_NAME -U $USERNAME -P $PASSWORD -i database-complete-schema-part2.sql

if [ $? -ne 0 ]; then
    echo "✗ Error creating additional tables"
    exit 1
fi
echo "✓ Additional tables created"
echo ""

echo "Step 3: Seeding initial data..."
sqlcmd -S $SERVER_NAME -U $USERNAME -P $PASSWORD -i database-seed-data.sql

if [ $? -ne 0 ]; then
    echo "✗ Error seeding data"
    exit 1
fi
echo "✓ Initial data seeded"
echo ""

# Verify setup
echo "Verifying setup..."
sqlcmd -S $SERVER_NAME -U $USERNAME -P $PASSWORD -d $DATABASE_NAME -Q "
    SELECT 
        (SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE') AS TableCount,
        (SELECT COUNT(*) FROM Outcomes) AS OutcomeCount,
        (SELECT COUNT(*) FROM Questions) AS QuestionCount,
        (SELECT COUNT(*) FROM SMSTemplates) AS TemplateCount;
"

echo ""
echo "========================================"
echo "Setup Complete!"
echo "========================================"
echo ""
echo "Database: $DATABASE_NAME"
echo ""
echo "Next Steps:"
echo "1. Update appsettings.json with connection string"
echo "2. Create admin user (see QUICK-START-DATABASE.md)"
echo "3. Run: cd src/VetSuccess.Api && dotnet run"
echo "4. Access Swagger: https://localhost:5001/swagger"
echo ""
