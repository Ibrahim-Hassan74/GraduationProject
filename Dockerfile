# ========== BUILD STAGE ==========
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files first (for layer caching)
COPY GraduationProject.slnx .
COPY SmartMicrobus.API/SmartMicrobus.API.csproj SmartMicrobus.API/
COPY SmartMicrobus.Core/SmartMicrobus.Core.csproj SmartMicrobus.Core/
COPY SmartMicrobus.Infrastructure/SmartMicrobus.Infrastructure.csproj SmartMicrobus.Infrastructure/

# Restore dependencies
RUN dotnet restore GraduationProject.slnx

# Copy everything else
COPY . .

# Build and publish
RUN dotnet publish SmartMicrobus.API/SmartMicrobus.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ========== RUNTIME STAGE ==========
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install ICU for globalization (Arabic/English localization)
RUN apt-get update && apt-get install -y --no-install-recommends \
    libicu-dev \
    && rm -rf /var/lib/apt/lists/*

EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "SmartMicrobus.API.dll"]
