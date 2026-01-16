# Multi-stage Dockerfile for VetSuccess .NET API

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["VetSuccess.sln", "./"]
COPY ["src/VetSuccess.Api/VetSuccess.Api.csproj", "src/VetSuccess.Api/"]
COPY ["src/VetSuccess.Application/VetSuccess.Application.csproj", "src/VetSuccess.Application/"]
COPY ["src/VetSuccess.Domain/VetSuccess.Domain.csproj", "src/VetSuccess.Domain/"]
COPY ["src/VetSuccess.Infrastructure/VetSuccess.Infrastructure.csproj", "src/VetSuccess.Infrastructure/"]
COPY ["src/VetSuccess.Shared/VetSuccess.Shared.csproj", "src/VetSuccess.Shared/"]

# Restore dependencies
RUN dotnet restore "VetSuccess.sln"

# Copy all source files
COPY . .

# Build the application
WORKDIR "/src/src/VetSuccess.Api"
RUN dotnet build "VetSuccess.Api.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "VetSuccess.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Create non-root user
RUN groupadd -r appuser && useradd -r -g appuser appuser

# Copy published files
COPY --from=publish /app/publish .

# Set ownership
RUN chown -R appuser:appuser /app

# Switch to non-root user
USER appuser

# Configure Kestrel
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Expose port
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Start the application
ENTRYPOINT ["dotnet", "VetSuccess.Api.dll"]
