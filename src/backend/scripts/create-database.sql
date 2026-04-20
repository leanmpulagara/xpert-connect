-- Create the development database
-- Run this script as the postgres superuser

CREATE DATABASE xpertconnect_dev
    WITH
    OWNER = postgres
    ENCODING = 'UTF8'
    CONNECTION LIMIT = -1;

-- Grant privileges
GRANT ALL PRIVILEGES ON DATABASE xpertconnect_dev TO postgres;
