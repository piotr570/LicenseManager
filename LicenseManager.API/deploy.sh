#!/bin/sh
set -eux

echo "------ Starting deploy.sh ------"
echo "DB_HOST=$DB_HOST, DB_PORT=$DB_PORT, DB_USER=$DB_USER, DB_NAME=$DB_NAME"

until pg_isready -h "$DB_HOST" -p "$DB_PORT" -U "$DB_USER" -d "$DB_NAME"; do
  echo "Waiting for database"
  sleep 2
done

echo "Database is ready. Applying migrations"

cd /app/LicenseManager.API
export ASPNETCORE_ENVIRONMENT=${ASPNETCORE_ENVIRONMENT:-Development}
dotnet ef database update --no-build --project LicenseManager.API.csproj --startup-project LicenseManager.API.csproj --configuration Release

echo "EF migration finished, starting API"
cd /app
exec dotnet LicenseManager.API.dll
