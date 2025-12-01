#!/bin/sh
set -eux

echo "------ Starting deploy.sh ------"
echo "DB_HOST=$DB_HOST, DB_PORT=$DB_PORT, DB_USER=$DB_USER, DB_NAME=$DB_NAME"

until pg_isready -h "$DB_HOST" -p "$DB_PORT" -U "$DB_USER" -d "$DB_NAME"; do
  echo "Waiting for database"
  sleep 2
done

echo "Database is ready. Applying migrations"

cd /app

# Apply migrations for Licenses module
echo "Applying migrations for Licenses module..."
dotnet ef database update \
  --project /app/LicenseManager.Licenses/LicenseManager.Licenses.csproj \
  --startup-project /app/LicenseManager.API/LicenseManager.API.csproj \
  --context LicensesDbContext \
  --configuration Release

# Apply migrations for Users module
echo "Applying migrations for Users module..."
dotnet ef database update \
  --project /app/LicenseManager.Users/LicenseManager.Users.csproj \
  --startup-project /app/LicenseManager.API/LicenseManager.API.csproj \
  --context UsersDbContext \
  --configuration Release

echo "All migrations finished, starting API"
exec dotnet LicenseManager.API.dll
