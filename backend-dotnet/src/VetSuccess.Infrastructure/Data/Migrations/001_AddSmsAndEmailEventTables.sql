-- Migration: Add SMS and Email Event Tables
-- Date: 2025-01-16
-- Description: Creates tables for SMSEvent, UpdatesEmailEvent, and SMSTemplate entities

-- Create sms_events table
CREATE TABLE IF NOT EXISTS sms_events (
    uuid UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    send_at TIMESTAMP WITH TIME ZONE NOT NULL,
    status VARCHAR(20) NOT NULL DEFAULT 'PENDING',
    context JSONB NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

-- Create index on send_at for efficient querying
CREATE INDEX IF NOT EXISTS idx_sms_events_send_at ON sms_events(send_at);

-- Create index on status for filtering
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

-- Create index on practice_id for efficient lookups
CREATE INDEX IF NOT EXISTS idx_updates_email_events_practice_id ON updates_email_events(practice_id);

-- Create index on status for filtering
CREATE INDEX IF NOT EXISTS idx_updates_email_events_status ON updates_email_events(status);

-- Create index on created_at for date-based queries
CREATE INDEX IF NOT EXISTS idx_updates_email_events_created_at ON updates_email_events(created_at);

-- Create sms_templates table
CREATE TABLE IF NOT EXISTS sms_templates (
    uuid UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    keywords VARCHAR(500) NOT NULL,
    template TEXT NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

-- Create unique index on keywords
CREATE UNIQUE INDEX IF NOT EXISTS idx_sms_templates_keywords ON sms_templates(keywords);

-- Add comments for documentation
COMMENT ON TABLE sms_events IS 'Stores scheduled SMS sending events with context';
COMMENT ON TABLE updates_email_events IS 'Stores daily email update events for practices';
COMMENT ON TABLE sms_templates IS 'Stores SMS templates with keyword matching';

COMMENT ON COLUMN sms_events.send_at IS 'Scheduled time to send the SMS';
COMMENT ON COLUMN sms_events.status IS 'Event status: PENDING, IN_PROGRESS';
COMMENT ON COLUMN sms_events.context IS 'JSON context containing SMS details (from, to, practice_id, sms_history_id, text)';

COMMENT ON COLUMN updates_email_events.status IS 'Event status: NO_FILES, PENDING, SENT, ERROR';
COMMENT ON COLUMN updates_email_events.practice_id IS 'Reference to practice ODU ID';
COMMENT ON COLUMN updates_email_events.file_paths IS 'JSON array of blob storage file paths';
COMMENT ON COLUMN updates_email_events.error_message IS 'Error message if status is ERROR';

COMMENT ON COLUMN sms_templates.keywords IS 'Comma-separated keywords for template matching';
COMMENT ON COLUMN sms_templates.template IS 'SMS template text with placeholders';
