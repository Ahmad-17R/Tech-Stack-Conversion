-- Migration: Seed SMS Templates
-- Date: 2025-01-16
-- Description: Inserts default SMS templates for common scenarios

-- Insert default SMS templates
INSERT INTO sms_templates (uuid, keywords, template, created_at, updated_at)
VALUES 
    (
        gen_random_uuid(),
        'appointment,reminder,upcoming',
        'Hi {client_name}, this is a reminder about {patient_name}''s appointment at {practice_name} on {appointment_date}. Call us at {practice_phone} if you need to reschedule.',
        NOW(),
        NOW()
    ),
    (
        gen_random_uuid(),
        'followup,checkup,follow-up',
        'Hi {client_name}, it''s time for {patient_name}''s follow-up visit at {practice_name}. Please call {practice_phone} to schedule an appointment.',
        NOW(),
        NOW()
    ),
    (
        gen_random_uuid(),
        'vaccination,vaccine,shots',
        'Hi {client_name}, {patient_name} is due for vaccinations at {practice_name}. Call {practice_phone} to schedule. Keep your pet protected!',
        NOW(),
        NOW()
    ),
    (
        gen_random_uuid(),
        'wellness,checkup,annual',
        'Hi {client_name}, it''s time for {patient_name}''s annual wellness exam at {practice_name}. Call {practice_phone} to book your appointment.',
        NOW(),
        NOW()
    ),
    (
        gen_random_uuid(),
        'dental,teeth,cleaning',
        'Hi {client_name}, {patient_name} is due for a dental checkup at {practice_name}. Healthy teeth mean a healthy pet! Call {practice_phone} to schedule.',
        NOW(),
        NOW()
    ),
    (
        gen_random_uuid(),
        'medication,prescription,refill',
        'Hi {client_name}, {patient_name}''s medication is ready for pickup at {practice_name}. Call {practice_phone} if you have questions.',
        NOW(),
        NOW()
    ),
    (
        gen_random_uuid(),
        'results,lab,test',
        'Hi {client_name}, {patient_name}''s test results are in. Please call {practice_name} at {practice_phone} to discuss.',
        NOW(),
        NOW()
    ),
    (
        gen_random_uuid(),
        'general,default,standard',
        'Hi {client_name}, this is {practice_name}. We have an update about {patient_name}. Please call us at {practice_phone}.',
        NOW(),
        NOW()
    )
ON CONFLICT (keywords) DO NOTHING;

-- Add comment
COMMENT ON TABLE sms_templates IS 'Default SMS templates seeded on 2025-01-16';
