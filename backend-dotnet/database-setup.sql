-- VetSuccess Database Setup Script
-- This script creates the complete database schema for the VetSuccess application
-- Run this script on a SQL Server instance to set up the database

USE master;
GO

-- Create database if it doesn't exist
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'VetSuccessDb')
BEGIN
    CREATE DATABASE VetSuccessDb;
    PRINT 'Database VetSuccessDb created successfully';
END
ELSE
BEGIN
    PRINT 'Database VetSuccessDb already exists';
END
GO

USE VetSuccessDb;
GO

-- Enable snapshot isolation for better concurrency
ALTER DATABASE VetSuccessDb SET ALLOW_SNAPSHOT_ISOLATION ON;
ALTER DATABASE VetSuccessDb SET READ_COMMITTED_SNAPSHOT ON;
GO

PRINT 'VetSuccess Database Setup Complete';
PRINT 'Next steps:';
PRINT '1. Update connection string in appsettings.json';
PRINT '2. Run: dotnet ef database update';
PRINT '3. Or run migrations manually from /Data/Migrations folder';
GO
