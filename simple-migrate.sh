#!/bin/bash
set -e

echo "[SIMPLE-MIGRATION] Starting database migration process..."

# Create database if it doesn't exist
DB_PASSWORD=${DB_PASSWORD:-FastFood2025}
DB_NAME=FastFood
DB_SERVER=db
DB_USER=sa

# Use the SQL Server tools to create the database if needed
/opt/mssql-tools18/bin/sqlcmd -S ${DB_SERVER} -U ${DB_USER} -P ${DB_PASSWORD} -Q "IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = '${DB_NAME}') BEGIN CREATE DATABASE [${DB_NAME}]; PRINT 'Database ${DB_NAME} created.'; END"

# Export the connection string for EF Core
export ConnectionStrings__DefaultConnection="Server=${DB_SERVER};Database=${DB_NAME};User Id=${DB_USER};Password=${DB_PASSWORD};TrustServerCertificate=true;"

# Try to run migrations using EF Core
cd /app/src/FastFood.Api
dotnet ef database update

echo "[SIMPLE-MIGRATION] Database migration completed!"
