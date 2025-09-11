-- Optimize golden_master_template database for template usage
-- This script runs after all migrations are complete

\echo 'Starting template database optimization...'

-- Connect to the template database
\c golden_master_template

-- Create golden master schema as a template for schema-based isolation
DROP SCHEMA IF EXISTS golden_master_schema CASCADE;
CREATE SCHEMA golden_master_schema;

-- Copy all objects from public schema to golden_master_schema for template use
DO $$
DECLARE
    rec RECORD;
    sql_stmt TEXT;
BEGIN
    -- Copy tables structure and data from public to golden_master_schema
    FOR rec IN 
        SELECT schemaname, tablename 
        FROM pg_tables 
        WHERE schemaname = 'public'
    LOOP
        sql_stmt := format('CREATE TABLE golden_master_schema.%I (LIKE public.%I INCLUDING ALL)',
            rec.tablename, rec.tablename);
        EXECUTE sql_stmt;
        
        sql_stmt := format('INSERT INTO golden_master_schema.%I SELECT * FROM public.%I',
            rec.tablename, rec.tablename);
        EXECUTE sql_stmt;
    END LOOP;
    
    -- Copy sequences and set their values
    FOR rec IN 
        SELECT schemaname, sequencename 
        FROM pg_sequences 
        WHERE schemaname = 'public'
    LOOP
        sql_stmt := format('CREATE SEQUENCE golden_master_schema.%I',
            rec.sequencename);
        EXECUTE sql_stmt;
        
        sql_stmt := format('SELECT setval(%L, (SELECT last_value FROM public.%I))',
            'golden_master_schema.' || rec.sequencename, rec.sequencename);
        EXECUTE sql_stmt;
    END LOOP;
    
    -- Copy functions and procedures
    FOR rec IN 
        SELECT p.proname, p.oid
        FROM pg_proc p
        JOIN pg_namespace n ON p.pronamespace = n.oid
        WHERE n.nspname = 'public'
    LOOP
        sql_stmt := pg_get_functiondef(rec.oid);
        sql_stmt := replace(sql_stmt, 'public.', 'golden_master_schema.');
        EXECUTE sql_stmt;
    END LOOP;
END
$$;

\echo 'Golden master schema template created.'

-- Analyze all tables to update statistics for better performance when creating from template
ANALYZE;

-- Vacuum to clean up any fragmentation and update visibility maps
VACUUM;

-- Reindex to ensure optimal index performance in new databases created from template
REINDEX DATABASE golden_master_template;

\echo 'Database analysis and cleanup completed.'

-- Connect back to postgres database for template configuration
\c postgres

-- Set connection limit to prevent excessive connections to template
ALTER DATABASE golden_master_template WITH CONNECTION LIMIT 3;

-- Set template database to read-only to prevent accidental modifications
-- This doesn't affect CREATE DATABASE WITH TEMPLATE operations
ALTER DATABASE golden_master_template WITH is_template = true;

\echo 'Template database configuration completed.'
\echo 'Golden master template is now optimized and ready for use.'