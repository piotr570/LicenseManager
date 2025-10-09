# Base image for the build stage
FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY ["LicenseManager.API/LicenseManager.API.csproj", "LicenseManager.API/"]
COPY ["LicenseManager.Application/LicenseManager.Application.csproj", "LicenseManager.Application/"]
COPY ["LicenseManager.Domain/LicenseManager.Domain.csproj", "LicenseManager.Domain/"]
COPY ["LicenseManager.Infrastructure/LicenseManager.Infrastructure.csproj", "LicenseManager.Infrastructure/"]
COPY ["LicenseManager.Notification/LicenseManager.Notification.csproj", "LicenseManager.Notification/"]

RUN dotnet restore "LicenseManager.API/LicenseManager.API.csproj"

# Copy the entire solution's source code
COPY . .

# Build the application
WORKDIR "/src/LicenseManager.API"
RUN dotnet build "LicenseManager.API.csproj" -c Release

# Publish the application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "LicenseManager.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Runtime image to host the application
FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/sdk:9.0 AS final
WORKDIR /app

# Install PostgreSQL client tools for pg_isready
RUN apt-get update && apt-get install -y postgresql-client && rm -rf /var/lib/apt/lists/*

# Copy the published application to the runtime image
COPY --from=publish /app/publish .

# Copy source files and .csproj for migrations
COPY LicenseManager.API /app/LicenseManager.API
COPY LicenseManager.Domain /app/LicenseManager.Domain
COPY LicenseManager.Application /app/LicenseManager.Application
COPY LicenseManager.Infrastructure /app/LicenseManager.Infrastructure
COPY LicenseManager.Notification /app/LicenseManager.Notification

# Copy deploy.sh into the image
COPY LicenseManager.API/deploy.sh ./deploy.sh
RUN chmod 755 ./deploy.sh

# Copy build artifacts for migrations
COPY --from=build /src/LicenseManager.API/obj /app/LicenseManager.API/obj
COPY --from=build /src/LicenseManager.API/bin /app/LicenseManager.API/bin
COPY --from=build /src/LicenseManager.Domain/obj /app/LicenseManager.Domain/obj
COPY --from=build /src/LicenseManager.Domain/bin /app/LicenseManager.Domain/bin
COPY --from=build /src/LicenseManager.Application/obj /app/LicenseManager.Application/obj
COPY --from=build /src/LicenseManager.Application/bin /app/LicenseManager.Application/bin
COPY --from=build /src/LicenseManager.Infrastructure/obj /app/LicenseManager.Infrastructure/obj
COPY --from=build /src/LicenseManager.Infrastructure/bin /app/LicenseManager.Infrastructure/bin
COPY --from=build /src/LicenseManager.Notification/obj /app/LicenseManager.Notification/obj
COPY --from=build /src/LicenseManager.Notification/bin /app/LicenseManager.Notification/bin

# Use a non-root user for enhanced security (optional)
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

# Install dotnet-ef CLI tool for appuser
RUN dotnet tool install --global dotnet-ef --version 9.0.2
ENV PATH="$PATH:/home/appuser/.dotnet/tools"

# Expose the HTTP port and set the entrypoint
EXPOSE 5666
ENV ASPNETCORE_URLS=http://+:5666
ENTRYPOINT ["./deploy.sh"]
