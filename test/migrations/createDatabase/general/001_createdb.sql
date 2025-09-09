-- Create the verenigingsregister database
-- This database will be used for running the application in development
-- The schema will be applied from the scripts in runAfterCreateDatabase folder

-- Drop the database if it exists (with force to disconnect any active connections)
DROP DATABASE IF EXISTS verenigingsregister WITH (FORCE);

-- Create the new database
CREATE DATABASE verenigingsregister
    WITH 
    OWNER = root
    ENCODING = 'UTF8'
    LC_COLLATE = 'en_US.utf8'
    LC_CTYPE = 'en_US.utf8'
    TABLESPACE = pg_default
    CONNECTION LIMIT = -1;