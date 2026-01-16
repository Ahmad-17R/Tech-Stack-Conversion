-- VetSuccess Complete Database Schema - Part 2
-- Appointments, Reminders, and Admin Tables

USE VetSuccessDb;
GO

PRINT '========================================';
PRINT 'Creating Appointment and Reminder Tables';
PRINT '========================================';

-- =============================================
-- APPOINTMENTS TABLE
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Appointments' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Appointments (
        AppointmentOduId NVARCHAR(255) NOT NULL PRIMARY KEY,
        ServerOduId NVARCHAR(255) NULL,
        ClientOduId NVARCHAR(255) NULL,
        PatientOduId NVARCHAR(255) NULL,
        PracticeOduId NVARCHAR(255) NULL,
        AppointmentDateTime DATETIME2 NULL,
        AppointmentType NVARCHAR(100) NULL,
        Status NVARCHAR(50) NULL,
        Notes NVARCHAR(MAX) NULL,
        Duration INT NULL,
        OduCreatedAt DATETIME2 NULL,
        OduUpdatedAt DATETIME2 NULL,
        ExtractorCreatedAt DATETIME2 NULL,
        ExtractorUpdatedAt DATETIME2 NULL,
        ExtractorRemovedAt DATETIME2 NULL,
        DataSource NVARCHAR(255) NULL,
        AppCreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        AppUpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        Uuid UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
        
        CONSTRAINT FK_Appointments_Servers FOREIGN KEY (ServerOduId) 
            REFERENCES dbo.Servers(ServerOduId) ON DELETE SET NULL,
        CONSTRAINT FK_Appointments_Clients FOREIGN KEY (ClientOduId) 
            REFERENCES dbo.Clients(ClientOduId) ON DELETE CASCADE,
        CONSTRAINT FK_Appointments_Patients FOREIGN KEY (PatientOduId) 
            REFERENCES dbo.Patients(PatientOduId) ON DELETE CASCADE,
        CONSTRAINT FK_Appointments_Practices FOREIGN KEY (PracticeOduId) 
            REFERENCES dbo.Practices(PracticeOduId) ON DELETE CASCADE
    );
    
    CREATE INDEX IX_Appointments_ClientOduId ON dbo.Appointments(ClientOduId);
    CREATE INDEX IX_Appointments_PatientOduId ON dbo.Appointments(PatientOduId);
    CREATE INDEX IX_Appointments_PracticeOduId ON dbo.Appointments(PracticeOduId);
    CREATE INDEX IX_Appointments_AppointmentDateTime ON dbo.Appointments(AppointmentDateTime);
    CREATE INDEX IX_Appointments_Status ON dbo.Appointments(Status);
    CREATE INDEX IX_Appointments_Uuid ON dbo.Appointments(Uuid);
    PRINT 'Created Appointments table';
END
GO

-- =============================================
-- REMINDERS TABLE
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Reminders' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Reminders (
        ReminderOduId NVARCHAR(255) NOT NULL PRIMARY KEY,
        ServerOduId NVARCHAR(255) NULL,
        ClientOduId NVARCHAR(255) NULL,
        PatientOduId NVARCHAR(255) NULL,
        PracticeOduId NVARCHAR(255) NULL,
        ReminderType NVARCHAR(100) NULL,
        DateDue DATE NULL,
        Status NVARCHAR(50) NULL,
        Notes NVARCHAR(MAX) NULL,
        SmsStatus NVARCHAR(50) NULL,
        OduCreatedAt DATETIME2 NULL,
        OduUpdatedAt DATETIME2 NULL,
        ExtractorCreatedAt DATETIME2 NULL,
        ExtractorUpdatedAt DATETIME2 NULL,
        ExtractorRemovedAt DATETIME2 NULL,
        DataSource NVARCHAR(255) NULL,
        AppCreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        AppUpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        Uuid UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
        
        CONSTRAINT FK_Reminders_Servers FOREIGN KEY (ServerOduId) 
            REFERENCES dbo.Servers(ServerOduId) ON DELETE SET NULL,
        CONSTRAINT FK_Reminders_Clients FOREIGN KEY (ClientOduId) 
            REFERENCES dbo.Clients(ClientOduId) ON DELETE CASCADE,
        CONSTRAINT FK_Reminders_Patients FOREIGN KEY (PatientOduId) 
            REFERENCES dbo.Patients(PatientOduId) ON DELETE CASCADE,
        CONSTRAINT FK_Reminders_Practices FOREIGN KEY (PracticeOduId) 
            REFERENCES dbo.Practices(PracticeOduId) ON DELETE CASCADE
    );
    
    CREATE INDEX IX_Reminders_ClientOduId ON dbo.Reminders(ClientOduId);
    CREATE INDEX IX_Reminders_PatientOduId ON dbo.Reminders(PatientOduId);
    CREATE INDEX IX_Reminders_PracticeOduId ON dbo.Reminders(PracticeOduId);
    CREATE INDEX IX_Reminders_DateDue ON dbo.Reminders(DateDue);
    CREATE INDEX IX_Reminders_Status ON dbo.Reminders(Status);
    CREATE INDEX IX_Reminders_SmsStatus ON dbo.Reminders(SmsStatus);
    CREATE INDEX IX_Reminders_Uuid ON dbo.Reminders(Uuid);
    PRINT 'Created Reminders table';
END
GO

PRINT '========================================';
PRINT 'Creating Admin Tables';
PRINT '========================================';

-- =============================================
-- OUTCOMES TABLE
-- =============================================
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
    PRINT 'Created Outcomes table';
END
GO

-- =============================================
-- QUESTIONS TABLE
-- =============================================
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
    PRINT 'Created Questions table';
END
GO

-- =============================================
-- ANSWERS TABLE
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Answers' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Answers (
        Uuid UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        PracticeOduId NVARCHAR(255) NOT NULL,
        QuestionId UNIQUEIDENTIFIER NOT NULL,
        AnswerText NVARCHAR(MAX) NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        
        CONSTRAINT FK_Answers_Practices FOREIGN KEY (PracticeOduId) 
            REFERENCES dbo.Practices(PracticeOduId) ON DELETE CASCADE,
        CONSTRAINT FK_Answers_Questions FOREIGN KEY (QuestionId) 
            REFERENCES dbo.Questions(Uuid) ON DELETE CASCADE,
        CONSTRAINT UQ_Answers_Practice_Question UNIQUE (PracticeOduId, QuestionId)
    );
    
    CREATE INDEX IX_Answers_PracticeOduId ON dbo.Answers(PracticeOduId);
    CREATE INDEX IX_Answers_QuestionId ON dbo.Answers(QuestionId);
    PRINT 'Created Answers table';
END
GO

-- =============================================
-- PRACTICE SETTINGS TABLE
-- =============================================
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
        
        CONSTRAINT FK_PracticeSettings_Practices FOREIGN KEY (PracticeOduId) 
            REFERENCES dbo.Practices(PracticeOduId) ON DELETE CASCADE
    );
    
    CREATE INDEX IX_PracticeSettings_PracticeOduId ON dbo.PracticeSettings(PracticeOduId);
    CREATE INDEX IX_PracticeSettings_IsSmsMailingEnabled ON dbo.PracticeSettings(IsSmsMailingEnabled);
    CREATE INDEX IX_PracticeSettings_IsEmailUpdatesEnabled ON dbo.PracticeSettings(IsEmailUpdatesEnabled);
    PRINT 'Created PracticeSettings table';
END
GO

PRINT '========================================';
PRINT 'Creating SMS and Email Tables';
PRINT '========================================';

-- =============================================
-- SMS TEMPLATES TABLE
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SMSTemplates' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.SMSTemplates (
        Uuid UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        Keywords NVARCHAR(500) NOT NULL,
        Template NVARCHAR(MAX) NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );
    
    CREATE INDEX IX_SMSTemplates_Keywords ON dbo.SMSTemplates(Keywords);
    PRINT 'Created SMSTemplates table';
END
GO

-- =============================================
-- SMS HISTORY TABLE
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SMSHistories' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.SMSHistories (
        Uuid UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        PracticeOduId NVARCHAR(255) NULL,
        ClientOduId NVARCHAR(255) NULL,
        EventContext NVARCHAR(MAX) NULL,
        SentAt DATETIME2 NULL,
        Status NVARCHAR(50) NOT NULL DEFAULT 'PENDING',
        Response NVARCHAR(MAX) NULL,
        ErrorMessage NVARCHAR(MAX) NULL,
        IsFollowed BIT NOT NULL DEFAULT 0,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        
        CONSTRAINT FK_SMSHistories_Practices FOREIGN KEY (PracticeOduId) 
            REFERENCES dbo.Practices(PracticeOduId) ON DELETE SET NULL,
        CONSTRAINT FK_SMSHistories_Clients FOREIGN KEY (ClientOduId) 
            REFERENCES dbo.Clients(ClientOduId) ON DELETE SET NULL
    );
    
    CREATE INDEX IX_SMSHistories_PracticeOduId ON dbo.SMSHistories(PracticeOduId);
    CREATE INDEX IX_SMSHistories_ClientOduId ON dbo.SMSHistories(ClientOduId);
    CREATE INDEX IX_SMSHistories_SentAt ON dbo.SMSHistories(SentAt);
    CREATE INDEX IX_SMSHistories_Status ON dbo.SMSHistories(Status);
    CREATE INDEX IX_SMSHistories_IsFollowed ON dbo.SMSHistories(IsFollowed);
    PRINT 'Created SMSHistories table';
END
GO

-- =============================================
-- SMS EVENTS TABLE
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SMSEvents' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.SMSEvents (
        Uuid UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        SendAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        Context NVARCHAR(MAX) NOT NULL,
        Status NVARCHAR(50) NOT NULL DEFAULT 'PENDING',
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );
    
    CREATE INDEX IX_SMSEvents_SendAt ON dbo.SMSEvents(SendAt);
    CREATE INDEX IX_SMSEvents_Status ON dbo.SMSEvents(Status);
    CREATE INDEX IX_SMSEvents_SendAt_Status ON dbo.SMSEvents(SendAt, Status);
    PRINT 'Created SMSEvents table';
END
GO

-- =============================================
-- UPDATES EMAIL EVENTS TABLE
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'UpdatesEmailEvents' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.UpdatesEmailEvents (
        Uuid UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        PracticeOduId NVARCHAR(255) NOT NULL,
        FilePaths NVARCHAR(MAX) NULL,
        Status NVARCHAR(50) NOT NULL DEFAULT 'PENDING',
        ErrorMessage NVARCHAR(MAX) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        
        CONSTRAINT FK_UpdatesEmailEvents_Practices FOREIGN KEY (PracticeOduId) 
            REFERENCES dbo.Practices(PracticeOduId) ON DELETE CASCADE
    );
    
    CREATE INDEX IX_UpdatesEmailEvents_PracticeOduId ON dbo.UpdatesEmailEvents(PracticeOduId);
    CREATE INDEX IX_UpdatesEmailEvents_Status ON dbo.UpdatesEmailEvents(Status);
    CREATE INDEX IX_UpdatesEmailEvents_CreatedAt ON dbo.UpdatesEmailEvents(CreatedAt);
    PRINT 'Created UpdatesEmailEvents table';
END
GO

PRINT 'All tables created successfully!';
PRINT '';
PRINT 'Next steps:';
PRINT '1. Run seed data script: database-seed-data.sql';
PRINT '2. Create admin user';
PRINT '3. Configure application connection string';
GO
