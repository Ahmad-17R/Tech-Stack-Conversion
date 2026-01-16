-- Apply Admin Migrations Script
-- Run this script to apply all admin-related migrations

USE VetSuccessDb;
GO

PRINT 'Starting Admin Migrations...';
PRINT '';

-- Migration 003: Create Admin Tables
PRINT 'Applying Migration 003: Create Admin Tables';
:r 003_CreateAdminTables.sql
PRINT '';

-- Migration 004: Seed Initial Data
PRINT 'Applying Migration 004: Seed Initial Data';
:r 004_SeedInitialData.sql
PRINT '';

PRINT 'All Admin Migrations Applied Successfully!';
GO
