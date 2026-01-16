-- Migration: 003_CreateAdminTables
-- Description: Creates tables for admin functionality (Users, Outcomes, Questions, Answers, PracticeSettings, SMSTemplates)
-- Date: 2026-01-16

-- Create Users table (extends ASP.NET Identity)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    -- Note: AspNetUsers table is created by Identity, we just add custom columns
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.AspNetUsers') AND name = 'Uuid')
    BEGIN
        ALTER TABLE dbo.AspNetUsers ADD Uuid UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID();
        ALTER TABLE dbo.AspNetUsers ADD CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE();
        ALTER TABLE dbo.AspNetUsers ADD UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE();
        ALTER TABLE dbo.AspNetUsers ADD RefreshToken NVARCHAR(500) NULL;
        ALTER TABLE dbo.AspNetUsers ADD RefreshTokenExpiryTime DATETIME2 NULL;
        
        CREATE INDEX IX_AspNetUsers_Uuid ON dbo.AspNetUsers(Uuid);
        CREATE INDEX IX_AspNetUsers_Email ON dbo.AspNetUsers(Email);
    END
END
GO

-- Create Outcomes table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Outcomes' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Outcomes (
        Uuid UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        OutcomeOduId NVARCHAR(255) NOT NULL UNIQUE,
        OutcomeName NVARCHAR(255) NOT NULL,
        Description NVARCHAR(MAX) NULL,
        RequiresFollowUp BIT NOT NULL DEFAULT 0,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );
    
    CREATE INDEX IX_Outcomes_OutcomeOduId ON dbo.Outcomes(OutcomeOduId);
    CREATE INDEX IX_Outcomes_OutcomeName ON dbo.Outcomes(OutcomeName);
END
GO

-- Create Questions table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Questions' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Questions (
        Uuid UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        QuestionText NVARCHAR(1000) NOT NULL,
        DisplayOrder INT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );
    
    CREATE INDEX IX_Questions_QuestionText ON dbo.Questions(QuestionText);
    CREATE INDEX IX_Questions_DisplayOrder ON dbo.Questions(DisplayOrder);
END
GO

-- Create Answers table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Answers' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Answers (
        Uuid UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        PracticeOduId NVARCHAR(255) NOT NULL,
        QuestionId UNIQUEIDENTIFIER NOT NULL,
        AnswerText NVARCHAR(MAX) NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        
        CONSTRAINT FK_Answers_Practices FOREIGN KEY (PracticeOduId) REFERENCES dbo.Practices(PracticeOduId) ON DELETE CASCADE,
        CONSTRAINT FK_Answers_Questions FOREIGN KEY (QuestionId) REFERENCES dbo.Questions(Uuid) ON DELETE CASCADE,
        CONSTRAINT UQ_Answers_Practice_Question UNIQUE (PracticeOduId, QuestionId)
    );
    
    CREATE INDEX IX_Answers_PracticeOduId ON dbo.Answers(PracticeOduId);
    CREATE INDEX IX_Answers_QuestionId ON dbo.Answers(QuestionId);
END
GO

-- Create PracticeSettings table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PracticeSettings' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.PracticeSettings (
        Uuid UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        PracticeOduId NVARCHAR(255) NOT NULL UNIQUE,
        IsSmsMailingEnabled BIT NOT NULL DEFAULT 0,
        IsEmailUpdatesEnabled BIT NOT NULL DEFAULT 0,
        SmsSendersPhone NVARCHAR(20) NULL,
        SmsScheduler NVARCHAR(100) NULL,
        SmsPracticeName NVARCHAR(100) NULL,
        SmsPhone NVARCHAR(20) NULL,
        SmsLink NVARCHAR(500) NULL,
        Email NVARCHAR(255) NULL,
        LaunchDate DATETIME2 NULL,
        StartDateForLaunch DATETIME2 NULL,
        EndDateForLaunch DATETIME2 NULL,
        SchedulerEmail NVARCHAR(255) NULL,
        RdoName NVARCHAR(100) NULL,
        RdoEmail NVARCHAR(255) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        
        CONSTRAINT FK_PracticeSettings_Practices FOREIGN KEY (PracticeOduId) REFERENCES dbo.Practices(PracticeOduId) ON DELETE CASCADE
    );
    
    CREATE INDEX IX_PracticeSettings_PracticeOduId ON dbo.PracticeSettings(PracticeOduId);
    CREATE INDEX IX_PracticeSettings_IsSmsMailingEnabled ON dbo.PracticeSettings(IsSmsMailingEnabled);
    CREATE INDEX IX_PracticeSettings_IsEmailUpdatesEnabled ON dbo.PracticeSettings(IsEmailUpdatesEnabled);
END
GO

-- Add IsArchived column to Practices table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Practices') AND name = 'IsArchived')
BEGIN
    ALTER TABLE dbo.Practices ADD IsArchived BIT NOT NULL DEFAULT 0;
    CREATE INDEX IX_Practices_IsArchived ON dbo.Practices(IsArchived);
END
GO

-- Update Patients table to add OutcomeOduId if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Patients') AND name = 'OutcomeOduId')
BEGIN
    ALTER TABLE dbo.Patients ADD OutcomeOduId NVARCHAR(255) NULL;
    ALTER TABLE dbo.Patients ADD OutcomeAt DATETIME2 NULL;
    
    CREATE INDEX IX_Patients_OutcomeOduId ON dbo.Patients(OutcomeOduId);
END
GO

-- Create Superuser role if it doesn't exist
IF NOT EXISTS (SELECT * FROM dbo.AspNetRoles WHERE Name = 'Superuser')
BEGIN
    INSERT INTO dbo.AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (NEWID(), 'Superuser', 'SUPERUSER', NEWID());
END
GO

PRINT 'Migration 003_CreateAdminTables completed successfully';
