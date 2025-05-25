#!/bin/bash
set -e

echo "[MIGRATION] Starting database migration process..."

# Use the environment variable for the password
DB_PASSWORD=${DB_PASSWORD:-FastFood2025}
DB_NAME=FastFood
DB_SERVER=db
DB_USER=sa

# Connection string for SQLCMD
SQLCMD_OPTS="-S ${DB_SERVER} -U ${DB_USER} -P ${DB_PASSWORD} -C -N"

# Function to check SQL Server availability
check_sql_server() {
  echo "[MIGRATION] Checking SQL Server availability..."
  /opt/mssql-tools18/bin/sqlcmd ${SQLCMD_OPTS} -d master -Q "SELECT 'SQL Server is ready'"
  return $?
}

# Wait for SQL Server to be ready (up to 5 minutes)
echo "[MIGRATION] Waiting for SQL Server to be ready..."
for i in {1..60}; do
  if check_sql_server; then
    echo "[OK] SQL Server is ready!"
    break
  else
    echo "[${i}/60] Waiting for SQL Server to be ready... (attempt ${i})"
    sleep 5
  fi
  
  if [ $i -eq 60 ]; then
    echo "[ERROR] Timeout waiting for SQL Server after 5 minutes"
    echo "[ERROR] Last error code: $?"
    exit 1
  fi
done

# Create database if it doesn't exist
echo "[MIGRATION] Checking if database '${DB_NAME}' exists..."
if ! /opt/mssql-tools18/bin/sqlcmd ${SQLCMD_OPTS} -d master -Q "IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = '${DB_NAME}') BEGIN CREATE DATABASE [${DB_NAME}]; PRINT 'Database ${DB_NAME} created.'; END"; then
  echo "[ERROR] Failed to check/create database"
  exit 1
fi

echo "[MIGRATION] Database '${DB_NAME}' is ready"

# Export the connection string for EF Core
export ConnectionStrings__DefaultConnection="Server=${DB_SERVER};Database=${DB_NAME};User Id=${DB_USER};Password=${DB_PASSWORD};TrustServerCertificate=true;"

echo "[MIGRATION] Connection string: Server=${DB_SERVER};Database=${DB_NAME};User Id=${DB_USER};Password=****;TrustServerCertificate=true;"

# Change to the source directory where the projects are located
cd /app

# Debug: List files in current directory
echo "[DEBUG] Current directory: $(pwd)"
echo "[DEBUG] Files in current directory:"
ls -la

# Debug: Check if src directory exists
echo "[DEBUG] Checking src directory:"
ls -la src

# Debug: Check if FastFood.Api directory exists
echo "[DEBUG] Checking FastFood.Api directory:"
ls -la src/FastFood.Api

# Debug: Check if FastFood.Infrastructure directory exists
echo "[DEBUG] Checking FastFood.Infrastructure directory:"
ls -la src/FastFood.Infrastructure

# Debug: Check if project files exist
echo "[DEBUG] Checking FastFood.Api.csproj:"
ls -la src/FastFood.Api/FastFood.Api.csproj

echo "[DEBUG] Checking FastFood.Infrastructure.csproj:"
ls -la src/FastFood.Infrastructure/FastFood.Infrastructure.csproj

# Change to the solution directory
cd /app

# Build the solution
echo "[MIGRATION] Building the solution..."
if ! dotnet build -c Release --no-restore; then
  echo "[ERROR] Failed to build the solution"
  exit 1
fi

# Run the migrations using the --migrate flag
echo "[MIGRATION] Running migrations..."
if ! dotnet run --project src/FastFood.Api/FastFood.Api.csproj -- --migrate; then
  echo "[MIGRATION] Failed to apply migrations via dotnet run"
  # Tentar uma abordagem alternativa usando o dotnet ef
  echo "[MIGRATION] Trying alternative approach with dotnet ef..."
  
  # Executar o script manual de migração
  if ! /manual-migrate.sh; then
    echo "[MIGRATION] Failed with manual migration script too"
    exit 1
  fi
fi

echo "[MIGRATION] Database setup complete!"
echo "[SUCCESS] Database migration completed successfully!"
