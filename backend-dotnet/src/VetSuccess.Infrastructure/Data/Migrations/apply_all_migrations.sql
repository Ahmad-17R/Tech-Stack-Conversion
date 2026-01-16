-- Master Migration Script
-- Date: 2025-01-16
-- Description: Applies all migrations in order

-- Enable UUID extension if not already enabled
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- Create migration tracking table
CREATE TABLE IF NOT EXISTS __migrations (
    id SERIAL PRIMARY KEY,
    migration_name VARCHAR(255) NOT NULL UNIQUE,
    applied_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    applied_by VARCHAR(255) NOT NULL DEFAULT CURRENT_USER
);

-- Function to check if migration was already applied
CREATE OR REPLACE FUNCTION is_migration_applied(migration_name VARCHAR)
RETURNS BOOLEAN AS $$
BEGIN
    RETURN EXISTS (SELECT 1 FROM __migrations WHERE __migrations.migration_name = $1);
END;
$$ LANGUAGE plpgsql;

-- Apply Migration 001: Add SMS and Email Event Tables
DO $$
BEGIN
    IF NOT is_migration_applied('001_AddSmsAndEmailEventTables') THEN
        RAISE NOTICE 'Applying migration: 001_AddSmsAndEmailEventTables';
        
        -- Create sms_events table
        CREATE TABLE IF NOT EXISTS sms_events (
            uuid UUID PRIMARY KEY DEFAULT gen_random_uuid(),
            send_at TIMESTAMP WITH TIME ZONE NOT NULL,
            status VARCHAR(20) NOT NULL DEFAULT 'PENDING',
            context JSONB NOT NULL,
            created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
            updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
        );

        CREATE INDEX IF NOT EXISTS idx_sms_events_send_at ON sms_events(send_at);
        CREATE INDEX IF NOT EXISTS idx_sms_events_status ON sms_events(status);

        -- Create updates_email_events table
        CREATE TABLE IF NOT EXISTS updates_email_events (
            uuid UUID PRIMARY KEY DEFAULT gen_random_uuid(),
            status VARCHAR(20) NOT NULL DEFAULT 'PENDING',
            practice_id VARCHAR(255) NOT NULL,
            file_paths JSONB NOT NULL DEFAULT '[]'::jsonb,
            error_message TEXT,
            created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
            updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
            CONSTRAINT fk_updates_email_events_practice 
                FOREIGN KEY (practice_id) 
                REFERENCES practices(practice_odu_id) 
                ON DELETE RESTRICT
        );

        CREATE INDEX IF NOT EXISTS idx_updates_email_events_practice_id ON updates_email_events(practice_id);
        CREATE INDEX IF NOT EXISTS idx_updates_email_events_status ON updates_email_events(status);
        CREATE INDEX IF NOT EXISTS idx_updates_email_events_created_at ON updates_email_events(created_at);

        -- Create sms_templates table
        CREATE TABLE IF NOT EXISTS sms_templates (
            uuid UUID PRIMARY KEY DEFAULT gen_random_uuid(),
            keywords VARCHAR(500) NOT NULL,
            template TEXT NOT NULL,
            created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
            updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
        );

        CREATE UNIQUE INDEX IF NOT EXISTS idx_sms_templates_keywords ON sms_templates(keywords);

        -- Add comments
        COMMENT ON TABLE sms_events IS 'Stores scheduled SMS sending events with context';
        COMMENT ON TABLE updates_email_events IS 'Stores daily email update events for practices';
        COMMENT ON TABLE sms_templates IS 'Stores SMS templates with keyword matching';

        -- Record migration
        INSERT INTO __migrations (migration_name) VALUES ('001_AddSmsAndEmailEventTables');
        RAISE NOTICE 'Migration 001_AddSmsAndEmailEventTables applied successfully';
    ELSE
        RAISE NOTICE 'Migration 001_AddSmsAndEmailEventTables already applied, skipping';
    END IF;
END $$;

-- Apply Migration 002: Seed SMS Templates
DO $$
BEGIN
    IF NOT is_migration_applied('002_SeedSmsTemplates') THEN
        RAISE NOTICE 'Applying migration: 002_SeedSmsTemplates';
        
        INSERT INTO sms_templates (uuid, keywords, template, created_at, updated_at)
        VALUES 
            (gen_random_uuid(), 'appointment,reminder,upcoming', 
             'Hi {client_name}, this is a reminder about {patient_name}''s appointment at {practice_name} on {appointment_date}. Call us at {practice_phone} if you need to reschedule.', 
             NOW(), NOW()),
            (gen_random_uuid(), 'followup,checkup,follow-up', 
             'Hi {client_name}, it''s time for {patient_name}''s follow-up visit at {practice_name}. Please call {practice_phone} to schedule an appointment.', 
             NOW(), NOW()),
            (gen_random_uuid(), 'vaccination,vaccine,shots', 
             'Hi {client_name}, {patient_name} is due for vaccinations at {practice_name}. Call {practice_phone} to schedule. Keep your pet protected!', 
             NOW(), NOW()),
            (gen_random_uuid(), 'wellness,checkup,annual', 
             'Hi {client_name}, it''s time for {patient_name}''s annual wellness exam at {practice_name}. Call {practice_phone} to book your appointment.', 
             NOW(), NOW()),
            (gen_random_uuid(), 'dental,teeth,cleaning', 
             'Hi {client_name}, {patient_name} is due for a dental checkup at {practice_name}. Healthy teeth mean a healthy pet! Call {practice_phone} to schedule.', 
             NOW(), NOW()),
            (gen_random_uuid(), 'medication,prescription,refill', 
             'Hi {client_name}, {patient_name}''s medication is ready for pickup at {practice_name}. Call {practice_phone} if you have questions.', 
             NOW(), NOW()),
            (gen_random_uuid(), 'results,lab,test', 
             'Hi {client_name}, {patient_name}''s test results are in. Please call {practice_name} at {practice_phone} to discuss.', 
             NOW(), NOW()),
            (gen_random_uuid(), 'general,default,standard', 
             'Hi {client_name}, this is {practice_name}. We have an update about {patient_name}. Please call us at {practice_phone}.', 
             NOW(), NOW())
        ON CONFLICT (keywords) DO NOTHING;

        -- Record migration
        INSERT INTO __migrations (migration_name) VALUES ('002_SeedSmsTemplates');
        RAISE NOTICE 'Migration 002_SeedSmsTemplates applied successfully';
    ELSE
        RAISE NOTICE 'Migration 002_SeedSmsTemplates already applied, skipping';
    END IF;
END $$;

-- Display migration status
SELECT 
    migration_name,
    applied_at,
    applied_by
FROM __migrations
ORDER BY id;

RAISE NOTICE 'All migrations applied successfully!';
