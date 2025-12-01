# Base image for the build stage
FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution file
COPY ["LicenseManager.sln", "."]
COPY ["global.json", "."]

# Copy all project files
COPY ["LicenseManager.API/LicenseManager.API.csproj", "LicenseManager.API/"]
COPY ["LicenseManager.Licenses/LicenseManager.Licenses.csproj", "LicenseManager.Licenses/"]
COPY ["LicenseManager.Users/LicenseManager.Users.csproj", "LicenseManager.Users/"]
COPY ["LicenseManager.Notification/LicenseManager.Notification.csproj", "LicenseManager.Notification/"]
COPY ["LicenseManager.Observability/LicenseManager.Observability.csproj", "LicenseManager.Observability/"]
COPY ["LicenseManager.SharedKernel/LicenseManager.SharedKernel.csproj", "LicenseManager.SharedKernel/"]
COPY ["LicenseManager.Tests.Application/LicenseManager.Tests.Application.csproj", "LicenseManager.Tests.Application/"]

# Restore dependencies
RUN dotnet restore "LicenseManager.sln"

# Copy entire source code
COPY . .

# Build the application
WORKDIR "/src/LicenseManager.API"
RUN dotnet build "LicenseManager.API.csproj" -c Release --no-restore

# Publish the application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "LicenseManager.API.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false \
    --no-restore

# Runtime image - use SDK so we can run dotnet ef for migrations
FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/sdk:9.0 AS final
WORKDIR /app

# Install PostgreSQL client tools
RUN apt-get update && apt-get install -y postgresql-client curl && rm -rf /var/lib/apt/lists/*

# Install dotnet-ef CLI globally
RUN dotnet tool install --global dotnet-ef --version 9.0.0

# Add dotnet tools to PATH
ENV PATH="/root/.dotnet/tools:${PATH}"

# Copy published application
COPY --from=publish /app/publish .

# Copy source files needed for EF migrations
COPY LicenseManager.API /app/LicenseManager.API
COPY LicenseManager.Licenses /app/LicenseManager.Licenses
COPY LicenseManager.Users /app/LicenseManager.Users
COPY LicenseManager.Notification /app/LicenseManager.Notification
COPY LicenseManager.Observability /app/LicenseManager.Observability
COPY LicenseManager.SharedKernel /app/LicenseManager.SharedKernel

# Copy deploy script
COPY LicenseManager.API/deploy.sh ./deploy.sh
RUN chmod +x ./deploy.sh

# Copy build artifacts for migrations
COPY --from=build /src/LicenseManager.API/obj /app/LicenseManager.API/obj
COPY --from=build /src/LicenseManager.API/bin /app/LicenseManager.API/bin
COPY --from=build /src/LicenseManager.Licenses/obj /app/LicenseManager.Licenses/obj
COPY --from=build /src/LicenseManager.Licenses/bin /app/LicenseManager.Licenses/bin
COPY --from=build /src/LicenseManager.Users/obj /app/LicenseManager.Users/obj
COPY --from=build /src/LicenseManager.Users/bin /app/LicenseManager.Users/bin

# Expose port
EXPOSE 5666

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=40s --retries=3 \
    CMD pg_isready -h licensemanager_db -U admin -d master || exit 1

# Run deploy script and start application
ENTRYPOINT ["./deploy.sh"]
