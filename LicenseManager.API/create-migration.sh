#!/bin/bash

STARTUP_PROJECT="LicenseManager.API"
MIGRATION_NAME=${1:-"Migration$(date +%s)"}

echo "Creating migration: $MIGRATION_NAME"

# Licenses Module
echo "Creating migration for Licenses..."
dotnet ef migrations add $MIGRATION_NAME \
  --project LicenseManager.Licenses \
  --startup-project $STARTUP_PROJECT \
  --context LicensesDbContext

# Users Module
echo "Creating migration for Users..."
dotnet ef migrations add $MIGRATION_NAME \
  --project LicenseManager.Users \
  --startup-project $STARTUP_PROJECT \
  --context UsersDbContext

echo "Done!"
