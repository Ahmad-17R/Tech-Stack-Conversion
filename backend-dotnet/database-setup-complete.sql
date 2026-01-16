-- VetSuccess Complete Database Setup
-- Master script that runs all setup steps in order
-- Run this script to set up the complete database

PRINT '========================================';
PRINT 'VetSuccess Database Complete Setup';
PRINT 'Starting at: ' + CONVERT(VARCHAR, GETDATE(), 120);
PRINT '========================================';
PRINT '';

-- Step 1: Create Database
PRINT 'Step 1: Creating Database...';
:r database-complete-schema.sql
PRINT '';

-- Step 2: Create Additional Tables
PRINT 'Step 2: Creating Additional Tables...';
:r database-complete-schema-part2.sql
PRINT '';

-- Step 3: Seed Initial Data
PRINT 'Step 3: Seeding Initial Data...';
:r database-seed-data.sql
PRINT '';

PRINT '========================================';
PRINT 'Database Setup Complete!';
PRINT 'Completed at: ' + CONVERT(VARCHAR, GETDATE(), 120);
PRINT '========================================';
PRINT '';
PRINT 'Database Summary:';
PRINT '- Database: VetSuccessDb';
PRINT '- Core Tables: 12';
PRINT '- Admin Tables: 7';
PRINT '- SMS/Email Tables: 4';
PRINT '- Total Tables: 23';
PRINT '';
PRINT 'Seeded Data:';
PRINT '- 10 Outcomes';
PRINT '- 10 Questions';
PRINT '- 8 SMS Templates';
PRINT '';
PRINT 'Next Steps:';
PRINT '1. Create admin user:';
PRINT '   - Use API: POST /api/v1/auth/register';
PRINT '   - Or run: database-create-admin-user.sql';
PRINT '2. Update appsettings.json with connection string';
PRINT '3. Run: dotnet run';
PRINT '4. Access Swagger: https://localhost:5001/swagger';
PRINT '';
GO
