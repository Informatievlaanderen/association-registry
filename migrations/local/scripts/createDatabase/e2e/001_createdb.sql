-- Create the golden_master_template database
-- This database will be used as a template for fast test database creation
-- The schema will be applied from the scripts in runAfterCreateDatabase folder
-- Drop the database if it exists (with force to disconnect any active connections)
DROP DATABASE IF EXISTS golden_master_template WITH (FORCE);

-- Create the new golden master template database
CREATE DATABASE golden_master_template
    WITH
    OWNER = root
    ENCODING = 'UTF8'
    LC_COLLATE = 'en_US.utf8'
    LC_CTYPE = 'en_US.utf8'
    TABLESPACE = pg_default
    CONNECTION LIMIT = -1;
