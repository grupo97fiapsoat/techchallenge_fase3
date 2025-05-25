@echo off
echo 🔍 Verificando status das migrations...
echo.

cd ..\src\FastFood.Api

echo 📋 Migrations disponíveis no código:
dotnet ef migrations list --project ..\FastFood.Infrastructure --startup-project .

echo.
echo 🗄️ Para verificar migrations aplicadas no banco, use:
echo docker exec -it [container_db] /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "%DB_PASSWORD%" -d FastFoodDb -Q "SELECT * FROM __EFMigrationsHistory"

pause
