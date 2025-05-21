# Base image for the build stage
FROM --platform=linux/arm64 mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY ["LicenseManager.API/LicenseManager.API.csproj", "LicenseManager.API/"]
COPY ["LicenseManager.Application/LicenseManager.Application.csproj", "LicenseManager.Application/"]
COPY ["LicenseManager.Domain/LicenseManager.Domain.csproj", "LicenseManager.Domain/"]
COPY ["LicenseManager.Infrastructure/LicenseManager.Infrastructure.csproj", "LicenseManager.Infrastructure/"]

RUN dotnet restore "LicenseManager.API/LicenseManager.API.csproj"

# Copy the entire solution's source code
COPY . .

# Build the application
WORKDIR "/src/LicenseManager.API"
RUN dotnet build "LicenseManager.API.csproj" -c Release -o /app/build

# Install EF Core CLI tools
RUN dotnet tool install --global dotnet-ef --version 9.0.2
ENV PATH="$PATH:/root/.dotnet/tools"

# Publish the application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "LicenseManager.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Runtime image to host the application
FROM --platform=linux/arm64 mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Copy the published application to the runtime image
COPY --from=publish /app/publish .

# Use a non-root user for enhanced security (optional)
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

# Expose the HTTP port and set the entrypoint
EXPOSE 5666
ENV ASPNETCORE_URLS=http://+:5666
ENTRYPOINT ["dotnet", "LicenseManager.API.dll"]