# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["PatientManagement.sln", "./"]
COPY ["src/PatientManagement.API/PatientManagement.API.csproj", "src/PatientManagement.API/"]
COPY ["src/PatientManagement.Application/PatientManagement.Application.csproj", "src/PatientManagement.Application/"]
COPY ["src/PatientManagement.Domain/PatientManagement.Domain.csproj", "src/PatientManagement.Domain/"]
COPY ["src/PatientManagement.Infrastructure/PatientManagement.Infrastructure.csproj", "src/PatientManagement.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "src/PatientManagement.API/PatientManagement.API.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/src/PatientManagement.API"
RUN dotnet build "PatientManagement.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "PatientManagement.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install SQL Server tools for backup operations
RUN apt-get update && apt-get install -y \
    curl \
    apt-transport-https \
    && curl https://packages.microsoft.com/keys/microsoft.asc | apt-key add - \
    && curl https://packages.microsoft.com/config/debian/11/prod.list > /etc/apt/sources.list.d/mssql-release.list \
    && apt-get update \
    && ACCEPT_EULA=Y apt-get install -y msodbcsql18 mssql-tools18 \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

# Add SQL tools to PATH
ENV PATH="$PATH:/opt/mssql-tools18/bin"

# Copy published files
COPY --from=publish /app/publish .

# Create directories for logs and backups
RUN mkdir -p /app/logs /app/backups

# Expose port
EXPOSE 80
EXPOSE 443

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
  CMD curl -f http://localhost:80/health || exit 1

ENTRYPOINT ["dotnet", "PatientManagement.API.dll"]
