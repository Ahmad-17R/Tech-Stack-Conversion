-- Rollback Migration: Add SMS and Email Event Tables
-- Date: 2025-01-16
-- Description: Drops tables for SMSEvent, UpdatesEmailEvent, and SMSTemplate entities

-- Drop tables in reverse order (respecting foreign key constraints)
DROP TABLE IF EXISTS sms_templates CASCADE;
DROP TABLE IF EXISTS updates_email_events CASCADE;
DROP TABLE IF EXISTS sms_events CASCADE;

-- Note: Indexes will be automatically dropped with the tables
