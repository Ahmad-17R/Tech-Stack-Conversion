-- Rollback Admin Migrations Script
-- Run this script to rollback all admin-related migrations

USE VetSuccessDb;
GO

PRINT 'Starting Admin Migrations Rollback...';
PRINT '';

-- Rollback Migration 004: Seed Initial Data
PRINT 'Rolling back Migration 004: Seed Initial Data';
:r 004_SeedInitialData_Rollback.sql
PRINT '';

-- Rollback Migration 003: Create Admin Tables
PRINT 'Rolling back Migration 003: Create Admin Tables';
:r 003_CreateAdminTables_Rollback.sql
PRINT '';

PRINT 'All Admin Migrations Rolled Back Successfully!';
GO
