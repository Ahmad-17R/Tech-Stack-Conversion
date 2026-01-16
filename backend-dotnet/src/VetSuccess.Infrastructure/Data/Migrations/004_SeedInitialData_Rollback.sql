-- Rollback Migration: 004_SeedInitialData
-- Description: Removes seeded initial data
-- Date: 2026-01-16

-- Remove seeded Questions
DELETE FROM dbo.Questions WHERE QuestionText IN (
    'What are your practice hours?',
    'Do you offer emergency services?',
    'What payment methods do you accept?',
    'Do you have parking available?',
    'What is your cancellation policy?',
    'Do you offer wellness plans?',
    'What types of pets do you treat?',
    'Do you have specialists on staff?',
    'What is your appointment scheduling process?',
    'Do you offer telemedicine consultations?'
);

-- Remove seeded Outcomes
DELETE FROM dbo.Outcomes WHERE OutcomeOduId IN (
    'OUTCOME_001', 'OUTCOME_002', 'OUTCOME_003', 'OUTCOME_004', 'OUTCOME_005',
    'OUTCOME_006', 'OUTCOME_007', 'OUTCOME_008', 'OUTCOME_009', 'OUTCOME_010'
);

PRINT 'Rollback 004_SeedInitialData completed successfully';
