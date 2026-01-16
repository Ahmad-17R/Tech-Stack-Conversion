-- VetSuccess Complete Database Schema
-- SQL Server Database Setup Script
-- This script creates the complete database schema for all operations

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

PRINT '========================================';
PRINT 'Creating Core Tables';
PRINT '========================================';

-- =============================================
-- SERVERS TABLE
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Servers' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Servers (
        ServerOduId NVARCHAR(255) NOT NULL PRIMARY KEY,
        ServerName NVARCHAR(255) NULL,
        OduCreatedAt DATETIME2 NULL,
        OduUpdatedAt DATETIME2 NULL,
        ExtractorCreatedAt DATETIME2 NULL,
        ExtractorUpdatedAt DATETIME2 NULL,
        ExtractorRemovedAt DATETIME2 NULL,
        DataSource NVARCHAR(255) NULL,
        AppCreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        AppUpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );
    
    CREATE INDEX IX_Servers_ServerName ON dbo.Servers(ServerName);
    PRINT 'Created Servers table';
END
GO

-- =============================================
-- PRACTICES TABLE
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Practices' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Practices (
        PracticeOduId NVARCHAR(255) NOT NULL PRIMARY KEY,
        ServerOduId NVARCHAR(255) NULL,
        PracticeName NVARCHAR(255) NULL,
        Address1 NVARCHAR(500) NULL,
        Address2 NVARCHAR(500) NULL,
        City NVARCHAR(100) NULL,
        State NVARCHAR(50) NULL,
        Country NVARCHAR(100) NULL,
        ZipCode NVARCHAR(20) NULL,
        Phone NVARCHAR(50) NULL,
        HasPimsConnection BIT NULL,
        Pims NVARCHAR(100) NULL,
        LatestExtractorUpdated DATETIME2 NULL,
        LatestTransaction DATETIME2 NULL,
        ServerImportFinished DATETIME2 NULL,
        PracticeUpdatedAt DATETIME2 NULL,
        IsArchived BIT NOT NULL DEFAULT 0,
        OduCreatedAt DATETIME2 NULL,
        OduUpdatedAt DATETIME2 NULL,
        ExtractorCreatedAt DATETIME2 NULL,
        ExtractorUpdatedAt DATETIME2 NULL,
        ExtractorRemovedAt DATETIME2 NULL,
        DataSource NVARCHAR(255) NULL,
        AppCreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        AppUpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        
        CONSTRAINT FK_Practices_Servers FOREIGN KEY (ServerOduId) 
            REFERENCES dbo.Servers(ServerOduId) ON DELETE SET NULL
    );
    
    CREATE INDEX IX_Practices_ServerOduId ON dbo.Practices(ServerOduId);
    CREATE INDEX IX_Practices_PracticeName ON dbo.Practices(PracticeName);
    CREATE INDEX IX_Practices_City ON dbo.Practices(City);
    CREATE INDEX IX_Practices_State ON dbo.Practices(State);
    CREATE INDEX IX_Practices_ZipCode ON dbo.Practices(ZipCode);
    CREATE INDEX IX_Practices_IsArchived ON dbo.Practices(IsArchived);
    PRINT 'Created Practices table';
END
GO

-- =============================================
-- CLIENTS TABLE
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Clients' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Clients (
        ClientOduId NVARCHAR(255) NOT NULL PRIMARY KEY,
        ServerOduId NVARCHAR(255) NULL,
        PimsEnteredDate DATE NULL,
        EarliestTransactionDate DATE NULL,
        EarliestOnlineTransactionDate DATETIME2 NULL,
        IsNewDate DATE NULL,
        OnlineAccountCreated DATETIME2 NULL,
        PimsId NVARCHAR(255) NULL,
        PimsIsDeleted BIT NULL,
        PimsIsInactive BIT NULL,
        PimsHasSuspendedReminders BIT NULL,
        FirstName NVARCHAR(255) NULL,
        LastName NVARCHAR(255) NULL,
        FullName NVARCHAR(511) NULL,
        UpperFullName NVARCHAR(511) NULL,
        IsOnline BIT NULL,
        IsInclinic BIT NULL,
        NewDateUpdatedAt DATETIME2 NULL,
        LatestTransactionDate DATE NULL,
        ClientRecordUpdatedAt DATETIME2 NULL,
        IsSafeToContact BIT NULL,
        IsHomePractice BIT NULL,
        OduCreatedAt DATETIME2 NULL,
        OduUpdatedAt DATETIME2 NULL,
        ExtractorCreatedAt DATETIME2 NULL,
        ExtractorUpdatedAt DATETIME2 NULL,
        ExtractorRemovedAt DATETIME2 NULL,
        DataSource NVARCHAR(255) NULL,
        AppCreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        AppUpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        
        CONSTRAINT FK_Clients_Servers FOREIGN KEY (ServerOduId) 
            REFERENCES dbo.Servers(ServerOduId) ON DELETE SET NULL
    );
    
    CREATE INDEX IX_Clients_ServerOduId ON dbo.Clients(ServerOduId);
    CREATE INDEX IX_Clients_FullName ON dbo.Clients(FullName);
    CREATE INDEX IX_Clients_UpperFullName ON dbo.Clients(UpperFullName);
    CREATE INDEX IX_Clients_FirstName ON dbo.Clients(FirstName);
    CREATE INDEX IX_Clients_LastName ON dbo.Clients(LastName);
    PRINT 'Created Clients table';
END
GO

-- =============================================
-- PATIENTS TABLE
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Patients' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Patients (
        PatientOduId NVARCHAR(255) NOT NULL PRIMARY KEY,
        ServerOduId NVARCHAR(255) NULL,
        PimsId NVARCHAR(255) NULL,
        PatientName NVARCHAR(255) NULL,
        Species NVARCHAR(100) NULL,
        Breed NVARCHAR(100) NULL,
        Gender NVARCHAR(20) NULL,
        DateOfBirth DATE NULL,
        DateOfDeath DATE NULL,
        IsDeceased BIT NULL,
        PimsIsDeleted BIT NULL,
        PimsIsInactive BIT NULL,
        Color NVARCHAR(100) NULL,
        Weight DECIMAL(10, 2) NULL,
        OutcomeOduId NVARCHAR(255) NULL,
        OutcomeAt DATETIME2 NULL,
        Comment NVARCHAR(MAX) NULL,
        OptOut BIT NULL DEFAULT 0,
        OduCreatedAt DATETIME2 NULL,
        OduUpdatedAt DATETIME2 NULL,
        ExtractorCreatedAt DATETIME2 NULL,
        ExtractorUpdatedAt DATETIME2 NULL,
        ExtractorRemovedAt DATETIME2 NULL,
        DataSource NVARCHAR(255) NULL,
        AppCreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        AppUpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        
        CONSTRAINT FK_Patients_Servers FOREIGN KEY (ServerOduId) 
            REFERENCES dbo.Servers(ServerOduId) ON DELETE SET NULL
    );
    
    CREATE INDEX IX_Patients_ServerOduId ON dbo.Patients(ServerOduId);
    CREATE INDEX IX_Patients_PatientName ON dbo.Patients(PatientName);
    CREATE INDEX IX_Patients_Species ON dbo.Patients(Species);
    CREATE INDEX IX_Patients_OutcomeOduId ON dbo.Patients(OutcomeOduId);
    CREATE INDEX IX_Patients_IsDeceased ON dbo.Patients(IsDeceased);
    PRINT 'Created Patients table';
END
GO

-- =============================================
-- CLIENT-PATIENT RELATIONSHIPS TABLE
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ClientPatientRelationships' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.ClientPatientRelationships (
        ClientPatientRelationshipOduId NVARCHAR(255) NOT NULL PRIMARY KEY,
        ServerOduId NVARCHAR(255) NULL,
        ClientOduId NVARCHAR(255) NULL,
        PatientOduId NVARCHAR(255) NULL,
        StartDate DATE NULL,
        EndDate DATE NULL,
        IsPrimary BIT NULL,
        Percentage DECIMAL(5, 2) NULL,
        RelationshipType NVARCHAR(100) NULL,
        OduCreatedAt DATETIME2 NULL,
        OduUpdatedAt DATETIME2 NULL,
        ExtractorCreatedAt DATETIME2 NULL,
        ExtractorUpdatedAt DATETIME2 NULL,
        ExtractorRemovedAt DATETIME2 NULL,
        DataSource NVARCHAR(255) NULL,
        AppCreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        AppUpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        
        CONSTRAINT FK_ClientPatientRelationships_Servers FOREIGN KEY (ServerOduId) 
            REFERENCES dbo.Servers(ServerOduId) ON DELETE SET NULL,
        CONSTRAINT FK_ClientPatientRelationships_Clients FOREIGN KEY (ClientOduId) 
            REFERENCES dbo.Clients(ClientOduId) ON DELETE CASCADE,
        CONSTRAINT FK_ClientPatientRelationships_Patients FOREIGN KEY (PatientOduId) 
            REFERENCES dbo.Patients(PatientOduId) ON DELETE CASCADE
    );
    
    CREATE INDEX IX_ClientPatientRelationships_ClientOduId ON dbo.ClientPatientRelationships(ClientOduId);
    CREATE INDEX IX_ClientPatientRelationships_PatientOduId ON dbo.ClientPatientRelationships(PatientOduId);
    PRINT 'Created ClientPatientRelationships table';
END
GO

-- =============================================
-- EMAILS TABLE
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Emails' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Emails (
        EmailOduId NVARCHAR(255) NOT NULL PRIMARY KEY,
        ServerOduId NVARCHAR(255) NULL,
        ClientOduId NVARCHAR(255) NULL,
        EmailType NVARCHAR(50) NULL,
        EmailAddress NVARCHAR(255) NULL,
        IsPreferred BIT NULL,
        OduCreatedAt DATETIME2 NULL,
        OduUpdatedAt DATETIME2 NULL,
        ExtractorCreatedAt DATETIME2 NULL,
        ExtractorUpdatedAt DATETIME2 NULL,
        ExtractorRemovedAt DATETIME2 NULL,
        DataSource NVARCHAR(255) NULL,
        AppCreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        AppUpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        Uuid UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
        
        CONSTRAINT FK_Emails_Servers FOREIGN KEY (ServerOduId) 
            REFERENCES dbo.Servers(ServerOduId) ON DELETE SET NULL,
        CONSTRAINT FK_Emails_Clients FOREIGN KEY (ClientOduId) 
            REFERENCES dbo.Clients(ClientOduId) ON DELETE CASCADE
    );
    
    CREATE INDEX IX_Emails_ClientOduId ON dbo.Emails(ClientOduId);
    CREATE INDEX IX_Emails_EmailAddress ON dbo.Emails(EmailAddress);
    CREATE INDEX IX_Emails_IsPreferred ON dbo.Emails(IsPreferred);
    CREATE INDEX IX_Emails_Uuid ON dbo.Emails(Uuid);
    PRINT 'Created Emails table';
END
GO

-- =============================================
-- PHONES TABLE
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Phones' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Phones (
        PhoneOduId NVARCHAR(255) NOT NULL PRIMARY KEY,
        ServerOduId NVARCHAR(255) NULL,
        ClientOduId NVARCHAR(255) NULL,
        PhoneNumber NVARCHAR(50) NULL,
        PhoneType NVARCHAR(50) NULL,
        IsPreferred BIT NULL,
        OduCreatedAt DATETIME2 NULL,
        OduUpdatedAt DATETIME2 NULL,
        ExtractorCreatedAt DATETIME2 NULL,
        ExtractorUpdatedAt DATETIME2 NULL,
        ExtractorRemovedAt DATETIME2 NULL,
        DataSource NVARCHAR(255) NULL,
        AppCreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        AppUpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        Uuid UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
        
        CONSTRAINT FK_Phones_Servers FOREIGN KEY (ServerOduId) 
            REFERENCES dbo.Servers(ServerOduId) ON DELETE SET NULL,
        CONSTRAINT FK_Phones_Clients FOREIGN KEY (ClientOduId) 
            REFERENCES dbo.Clients(ClientOduId) ON DELETE CASCADE
    );
    
    CREATE INDEX IX_Phones_ClientOduId ON dbo.Phones(ClientOduId);
    CREATE INDEX IX_Phones_PhoneNumber ON dbo.Phones(PhoneNumber);
    CREATE INDEX IX_Phones_IsPreferred ON dbo.Phones(IsPreferred);
    CREATE INDEX IX_Phones_Uuid ON dbo.Phones(Uuid);
    PRINT 'Created Phones table';
END
GO

-- =============================================
-- ADDRESSES TABLE
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Addresses' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Addresses (
        AddressOduId NVARCHAR(255) NOT NULL PRIMARY KEY,
        ServerOduId NVARCHAR(255) NULL,
        ClientOduId NVARCHAR(255) NULL,
        Line1 NVARCHAR(500) NULL,
        Line2 NVARCHAR(500) NULL,
        City NVARCHAR(100) NULL,
        State NVARCHAR(50) NULL,
        PostalCode NVARCHAR(20) NULL,
        AddressType NVARCHAR(50) NULL,
        IsPreferred BIT NULL,
        OduCreatedAt DATETIME2 NULL,
        OduUpdatedAt DATETIME2 NULL,
        ExtractorCreatedAt DATETIME2 NULL,
        ExtractorUpdatedAt DATETIME2 NULL,
        ExtractorRemovedAt DATETIME2 NULL,
        DataSource NVARCHAR(255) NULL,
        AppCreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        AppUpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        Uuid UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
        
        CONSTRAINT FK_Addresses_Servers FOREIGN KEY (ServerOduId) 
            REFERENCES dbo.Servers(ServerOduId) ON DELETE SET NULL,
        CONSTRAINT FK_Addresses_Clients FOREIGN KEY (ClientOduId) 
            REFERENCES dbo.Clients(ClientOduId) ON DELETE CASCADE
    );
    
    CREATE INDEX IX_Addresses_ClientOduId ON dbo.Addresses(ClientOduId);
    CREATE INDEX IX_Addresses_City ON dbo.Addresses(City);
    CREATE INDEX IX_Addresses_State ON dbo.Addresses(State);
    CREATE INDEX IX_Addresses_PostalCode ON dbo.Addresses(PostalCode);
    CREATE INDEX IX_Addresses_Uuid ON dbo.Addresses(Uuid);
    PRINT 'Created Addresses table';
END
GO

-- Continue in next part...
PRINT 'Core tables created successfully';
GO
