-- Master Rollback Script
-- Date: 2025-01-16
-- Description: Rolls back all migrations in reverse order

-- Rollback Migration 002: Seed SMS Templates
DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM __migrations WHERE migration_name = '002_SeedSmsTemplates') THEN
        RAISE NOTICE 'Rolling back migration: 002_SeedSmsTemplates';
        
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

        DELETE FROM __migrations WHERE migration_name = '002_SeedSmsTemplates';
        RAISE NOTICE 'Migration 002_SeedSmsTemplates rolled back successfully';
    ELSE
        RAISE NOTICE 'Migration 002_SeedSmsTemplates not applied, skipping rollback';
    END IF;
END $$;

-- Rollback Migration 001: Add SMS and Email Event Tables
DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM __migrations WHERE migration_name = '001_AddSmsAndEmailEventTables') THEN
        RAISE NOTICE 'Rolling back migration: 001_AddSmsAndEmailEventTables';
        
        DROP TABLE IF EXISTS sms_templates CASCADE;
        DROP TABLE IF EXISTS updates_email_events CASCADE;
        DROP TABLE IF EXISTS sms_events CASCADE;

        DELETE FROM __migrations WHERE migration_name = '001_AddSmsAndEmailEventTables';
        RAISE NOTICE 'Migration 001_AddSmsAndEmailEventTables rolled back successfully';
    ELSE
        RAISE NOTICE 'Migration 001_AddSmsAndEmailEventTables not applied, skipping rollback';
    END IF;
END $$;

-- Display remaining migrations
SELECT 
    migration_name,
    applied_at,
    applied_by
FROM __migrations
ORDER BY id;

RAISE NOTICE 'All migrations rolled back successfully!';
