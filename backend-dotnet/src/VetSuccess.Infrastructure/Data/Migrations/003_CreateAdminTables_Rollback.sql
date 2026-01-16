-- Rollback Migration: 003_CreateAdminTables
-- Description: Drops admin tables and columns
-- Date: 2026-01-16

-- Drop foreign key constraints first
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Answers_Practices')
    ALTER TABLE dbo.Answers DROP CONSTRAINT FK_Answers_Practices;

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Answers_Questions')
    ALTER TABLE dbo.Answers DROP CONSTRAINT FK_Answers_Questions;

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_PracticeSettings_Practices')
    ALTER TABLE dbo.PracticeSettings DROP CONSTRAINT FK_PracticeSettings_Practices;

-- Drop tables
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Answers' AND schema_id = SCHEMA_ID('dbo'))
    DROP TABLE dbo.Answers;

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'PracticeSettings' AND schema_id = SCHEMA_ID('dbo'))
    DROP TABLE dbo.PracticeSettings;

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Questions' AND schema_id = SCHEMA_ID('dbo'))
    DROP TABLE dbo.Questions;

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Outcomes' AND schema_id = SCHEMA_ID('dbo'))
    DROP TABLE dbo.Outcomes;

-- Remove columns from AspNetUsers
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.AspNetUsers') AND name = 'Uuid')
BEGIN
    ALTER TABLE dbo.AspNetUsers DROP COLUMN Uuid;
    ALTER TABLE dbo.AspNetUsers DROP COLUMN CreatedAt;
    ALTER TABLE dbo.AspNetUsers DROP COLUMN UpdatedAt;
    ALTER TABLE dbo.AspNetUsers DROP COLUMN RefreshToken;
    ALTER TABLE dbo.AspNetUsers DROP COLUMN RefreshTokenExpiryTime;
END

-- Remove columns from Practices
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Practices') AND name = 'IsArchived')
    ALTER TABLE dbo.Practices DROP COLUMN IsArchived;

-- Remove columns from Patients
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Patients') AND name = 'OutcomeOduId')
BEGIN
    ALTER TABLE dbo.Patients DROP COLUMN OutcomeOduId;
    ALTER TABLE dbo.Patients DROP COLUMN OutcomeAt;
END

-- Remove Superuser role
DELETE FROM dbo.AspNetRoles WHERE Name = 'Superuser';

PRINT 'Rollback 003_CreateAdminTables completed successfully';
