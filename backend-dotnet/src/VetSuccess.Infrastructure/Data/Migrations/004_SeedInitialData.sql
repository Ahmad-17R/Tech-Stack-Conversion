-- Migration: 004_SeedInitialData
-- Description: Seeds initial data for Outcomes and Questions
-- Date: 2026-01-16

-- Seed Outcomes
IF NOT EXISTS (SELECT * FROM dbo.Outcomes WHERE OutcomeOduId = 'OUTCOME_001')
BEGIN
    INSERT INTO dbo.Outcomes (Uuid, OutcomeOduId, OutcomeName, Description, RequiresFollowUp, CreatedAt, UpdatedAt)
    VALUES 
        (NEWID(), 'OUTCOME_001', 'Appointment Scheduled', 'Client has scheduled an appointment', 0, GETUTCDATE(), GETUTCDATE()),
        (NEWID(), 'OUTCOME_002', 'Not Interested', 'Client is not interested at this time', 0, GETUTCDATE(), GETUTCDATE()),
        (NEWID(), 'OUTCOME_003', 'Follow Up Required', 'Client needs follow up contact', 1, GETUTCDATE(), GETUTCDATE()),
        (NEWID(), 'OUTCOME_004', 'No Answer', 'No answer on phone call', 1, GETUTCDATE(), GETUTCDATE()),
        (NEWID(), 'OUTCOME_005', 'Wrong Number', 'Phone number is incorrect', 0, GETUTCDATE(), GETUTCDATE()),
        (NEWID(), 'OUTCOME_006', 'Voicemail Left', 'Left voicemail message', 1, GETUTCDATE(), GETUTCDATE()),
        (NEWID(), 'OUTCOME_007', 'Already Scheduled', 'Client already has appointment scheduled', 0, GETUTCDATE(), GETUTCDATE()),
        (NEWID(), 'OUTCOME_008', 'Deceased Pet', 'Pet has passed away', 0, GETUTCDATE(), GETUTCDATE()),
        (NEWID(), 'OUTCOME_009', 'Moved Away', 'Client has moved to different area', 0, GETUTCDATE(), GETUTCDATE()),
        (NEWID(), 'OUTCOME_010', 'Do Not Contact', 'Client requested no further contact', 0, GETUTCDATE(), GETUTCDATE());
    
    PRINT 'Seeded Outcomes';
END
GO

-- Seed Questions
IF NOT EXISTS (SELECT * FROM dbo.Questions WHERE QuestionText = 'What are your practice hours?')
BEGIN
    INSERT INTO dbo.Questions (Uuid, QuestionText, DisplayOrder, CreatedAt, UpdatedAt)
    VALUES 
        (NEWID(), 'What are your practice hours?', 1, GETUTCDATE(), GETUTCDATE()),
        (NEWID(), 'Do you offer emergency services?', 2, GETUTCDATE(), GETUTCDATE()),
        (NEWID(), 'What payment methods do you accept?', 3, GETUTCDATE(), GETUTCDATE()),
        (NEWID(), 'Do you have parking available?', 4, GETUTCDATE(), GETUTCDATE()),
        (NEWID(), 'What is your cancellation policy?', 5, GETUTCDATE(), GETUTCDATE()),
        (NEWID(), 'Do you offer wellness plans?', 6, GETUTCDATE(), GETUTCDATE()),
        (NEWID(), 'What types of pets do you treat?', 7, GETUTCDATE(), GETUTCDATE()),
        (NEWID(), 'Do you have specialists on staff?', 8, GETUTCDATE(), GETUTCDATE()),
        (NEWID(), 'What is your appointment scheduling process?', 9, GETUTCDATE(), GETUTCDATE()),
        (NEWID(), 'Do you offer telemedicine consultations?', 10, GETUTCDATE(), GETUTCDATE());
    
    PRINT 'Seeded Questions';
END
GO

PRINT 'Migration 004_SeedInitialData completed successfully';
