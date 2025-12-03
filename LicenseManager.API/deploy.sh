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

# Apply migrations using already built DLLs
echo "Applying migrations for Licenses module..."
dotnet ef database update \
  --context LicensesDbContext \
  --no-build \
  --configuration Release

echo "Applying migrations for Users module..."
dotnet ef database update \
  --context UsersDbContext \
  --no-build \
  --configuration Release

cd /app
echo "All migrations finished, starting API"
exec dotnet LicenseManager.API.dll
