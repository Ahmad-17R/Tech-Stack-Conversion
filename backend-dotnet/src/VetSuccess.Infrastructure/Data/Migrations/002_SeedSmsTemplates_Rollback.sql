-- Rollback Migration: Seed SMS Templates
-- Date: 2025-01-16
-- Description: Removes seeded SMS templates

-- Delete all seeded templates
DELETE FROM sms_templates 
WHERE keywords IN (
    'appointment,reminder,upcoming',
    'followup,checkup,follow-up',
    'vaccination,vaccine,shots',
    'wellness,checkup,annual',
    'dental,teeth,cleaning',
    'medication,prescription,refill',
    'results,lab,test',
    'general,default,standard'
);
