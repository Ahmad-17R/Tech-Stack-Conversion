-- VetSuccess Database Seed Data
-- This script seeds initial data for the application

USE VetSuccessDb;
GO

PRINT '========================================';
PRINT 'Seeding Initial Data';
PRINT '========================================';

-- =============================================
-- SEED OUTCOMES
-- =============================================
PRINT 'Seeding Outcomes...';

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
    
    PRINT 'Seeded 10 Outcomes';
END
ELSE
BEGIN
    PRINT 'Outcomes already seeded, skipping';
END
GO

-- =============================================
-- SEED QUESTIONS
-- =============================================
PRINT 'Seeding Questions...';

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
    
    PRINT 'Seeded 10 Questions';
END
ELSE
BEGIN
    PRINT 'Questions already seeded, skipping';
END
GO

-- =============================================
-- SEED SMS TEMPLATES
-- =============================================
PRINT 'Seeding SMS Templates...';

IF NOT EXISTS (SELECT * FROM dbo.SMSTemplates WHERE Keywords = 'appointment,reminder,upcoming')
BEGIN
    INSERT INTO dbo.SMSTemplates (Uuid, Keywords, Template, CreatedAt, UpdatedAt)
    VALUES 
        (NEWID(), 'appointment,reminder,upcoming', 
         'Hi, this is a reminder about your pet''s appointment at {practice_name} on {appointment_date}. Call us at {phone} if you need to reschedule.', 
         GETUTCDATE(), GETUTCDATE()),
        
        (NEWID(), 'followup,checkup,follow-up', 
         'Hi, it''s time for your pet''s follow-up visit at {practice_name}. Please call {phone} to schedule an appointment with {scheduler}.', 
         GETUTCDATE(), GETUTCDATE()),
        
        (NEWID(), 'vaccination,vaccine,shots', 
         'Hi, your pet is due for vaccinations at {practice_name}. Call {phone} to schedule. Keep your pet protected!', 
         GETUTCDATE(), GETUTCDATE()),
        
        (NEWID(), 'wellness,checkup,annual', 
         'Hi, it''s time for your pet''s annual wellness exam at {practice_name}. Call {phone} to book your appointment with {scheduler}.', 
         GETUTCDATE(), GETUTCDATE()),
        
        (NEWID(), 'dental,teeth,cleaning', 
         'Hi, your pet is due for a dental checkup at {practice_name}. Healthy teeth mean a healthy pet! Call {phone} to schedule.', 
         GETUTCDATE(), GETUTCDATE()),
        
        (NEWID(), 'medication,prescription,refill', 
         'Hi, your pet''s medication is ready for pickup at {practice_name}. Call {phone} if you have questions.', 
         GETUTCDATE(), GETUTCDATE()),
        
        (NEWID(), 'results,lab,test', 
         'Hi, your pet''s test results are in. Please call {practice_name} at {phone} to discuss with {scheduler}.', 
         GETUTCDATE(), GETUTCDATE()),
        
        (NEWID(), 'general,default,standard', 
         'Hi, this is {practice_name}. We have an update about your pet. Please call us at {phone}. Visit {link} for more info.', 
         GETUTCDATE(), GETUTCDATE());
    
    PRINT 'Seeded 8 SMS Templates';
END
ELSE
BEGIN
    PRINT 'SMS Templates already seeded, skipping';
END
GO

PRINT '';
PRINT '========================================';
PRINT 'Seed Data Complete!';
PRINT '========================================';
PRINT '';
PRINT 'Summary:';
PRINT '- 10 Outcomes';
PRINT '- 10 Questions';
PRINT '- 8 SMS Templates';
PRINT '';
PRINT 'Next steps:';
PRINT '1. Create admin user (see documentation)';
PRINT '2. Configure application settings';
PRINT '3. Start the application';
GO
