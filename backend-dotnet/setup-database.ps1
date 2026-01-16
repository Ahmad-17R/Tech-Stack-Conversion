# VetSuccess Database Setup Script
# PowerShell script to automate database setup

param(
    [string]$ServerName = "localhost",
    [string]$DatabaseName = "VetSuccessDb",
    [string]$Username = "",
    [string]$Password = "",
    [switch]$UseWindowsAuth = $true,
    [switch]$DropExisting = $false
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "VetSuccess Database Setup" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Build connection string
if ($UseWindowsAuth) {
    $connectionString = "Server=$ServerName;Database=master;Integrated Security=True;TrustServerCertificate=True"
    Write-Host "Using Windows Authentication" -ForegroundColor Green
} else {
    if ([string]::IsNullOrEmpty($Username) -or [string]::IsNullOrEmpty($Password)) {
        Write-Host "Error: Username and Password required for SQL Authentication" -ForegroundColor Red
        exit 1
    }
    $connectionString = "Server=$ServerName;Database=master;User Id=$Username;Password=$Password;TrustServerCertificate=True"
    Write-Host "Using SQL Authentication" -ForegroundColor Green
}

Write-Host "Server: $ServerName" -ForegroundColor Yellow
Write-Host "Database: $DatabaseName" -ForegroundColor Yellow
Write-Host ""

# Check if sqlcmd is available
try {
    $sqlcmdVersion = sqlcmd -?
    Write-Host "[OK] sqlcmd found" -ForegroundColor Green
} catch {
    Write-Host "[ERROR] sqlcmd not found. Please install SQL Server Command Line Utilities" -ForegroundColor Red
    Write-Host "Download from: https://docs.microsoft.com/en-us/sql/tools/sqlcmd-utility" -ForegroundColor Yellow
    exit 1
}

# Drop existing database if requested
if ($DropExisting) {
    Write-Host "Dropping existing database..." -ForegroundColor Yellow
    $dropScript = "USE master; IF EXISTS (SELECT name FROM sys.databases WHERE name = '$DatabaseName') BEGIN ALTER DATABASE [$DatabaseName] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [$DatabaseName]; PRINT 'Database dropped successfully'; END"
    
    if ($UseWindowsAuth) {
        Invoke-Expression "sqlcmd -S $ServerName -E -Q `"$dropScript`""
    } else {
        Invoke-Expression "sqlcmd -S $ServerName -U $Username -P $Password -Q `"$dropScript`""
    }
    Write-Host "Existing database dropped" -ForegroundColor Green
    Write-Host ""
}

# Run setup scripts
Write-Host "Step 1: Creating database and core tables..." -ForegroundColor Cyan
if ($UseWindowsAuth) {
    sqlcmd -S $ServerName -E -i "database-complete-schema.sql"
} else {
    sqlcmd -S $ServerName -U $Username -P $Password -i "database-complete-schema.sql"
}

if ($LASTEXITCODE -ne 0) {
    Write-Host "[ERROR] Error creating core tables" -ForegroundColor Red
    exit 1
}
Write-Host "[OK] Core tables created" -ForegroundColor Green
Write-Host ""

Write-Host "Step 2: Creating additional tables..." -ForegroundColor Cyan
if ($UseWindowsAuth) {
    sqlcmd -S $ServerName -E -i "database-complete-schema-part2.sql"
} else {
    sqlcmd -S $ServerName -U $Username -P $Password -i "database-complete-schema-part2.sql"
}

if ($LASTEXITCODE -ne 0) {
    Write-Host "[ERROR] Error creating additional tables" -ForegroundColor Red
    exit 1
}
Write-Host "[OK] Additional tables created" -ForegroundColor Green
Write-Host ""

Write-Host "Step 3: Seeding initial data..." -ForegroundColor Cyan
if ($UseWindowsAuth) {
    sqlcmd -S $ServerName -E -i "database-seed-data.sql"
} else {
    sqlcmd -S $ServerName -U $Username -P $Password -i "database-seed-data.sql"
}

if ($LASTEXITCODE -ne 0) {
    Write-Host "[ERROR] Error seeding data" -ForegroundColor Red
    exit 1
}
Write-Host "[OK] Initial data seeded" -ForegroundColor Green
Write-Host ""

# Verify setup
Write-Host "Verifying setup..." -ForegroundColor Cyan
$verifyScript = "USE $DatabaseName; SELECT (SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE') AS TableCount, (SELECT COUNT(*) FROM Outcomes) AS OutcomeCount, (SELECT COUNT(*) FROM Questions) AS QuestionCount, (SELECT COUNT(*) FROM SMSTemplates) AS TemplateCount;"

if ($UseWindowsAuth) {
    $result = Invoke-Expression "sqlcmd -S $ServerName -E -Q `"$verifyScript`" -h -1"
} else {
    $result = Invoke-Expression "sqlcmd -S $ServerName -U $Username -P $Password -Q `"$verifyScript`" -h -1"
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Setup Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Database: $DatabaseName" -ForegroundColor Yellow
Write-Host "Verification Results:" -ForegroundColor Yellow
Write-Host $result
Write-Host ""
Write-Host "Next Steps:" -ForegroundColor Cyan
Write-Host "1. Update appsettings.json with connection string" -ForegroundColor White
Write-Host "2. Create admin user (see QUICK-START-DATABASE.md)" -ForegroundColor White
Write-Host "3. Run: cd src/VetSuccess.Api; dotnet run" -ForegroundColor White
Write-Host "4. Access Swagger: https://localhost:5001/swagger" -ForegroundColor White
Write-Host ""
