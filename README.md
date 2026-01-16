# VetSuccess Backend - .NET Migration

A modern ASP.NET Core 8.0 backend for the VetSuccess Call Center application, migrated from Django/Python with full API compatibility.

## 📋 Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Tech Stack](#tech-stack)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Project Structure](#project-structure)
- [Configuration](#configuration)
- [Running the Application](#running-the-application)
- [API Documentation](#api-documentation)
- [Database](#database)
- [Background Jobs](#background-jobs)
- [Testing](#testing)
- [Deployment](#deployment)
- [Contributing](#contributing)

## 🎯 Overview

VetSuccess is a call center management system for veterinary practices. This backend provides:

- **Client & Patient Management** - Track clients, patients, and their relationships
- **SMS Communication** - Automated SMS campaigns via Dialpad integration
- **Appointment Tracking** - Monitor appointments and reminders
- **Outcome Management** - Record and analyze call outcomes
- **Practice Settings** - Configurable practice-specific settings
- **Background Jobs** - Automated SMS aggregation and email updates
- **Authentication** - JWT-based authentication with refresh tokens

## 🏗️ Architecture

The solution follows **Clean Architecture** principles with clear separation of concerns:

```
VetSuccess.sln
├── backend-dotnet/
│   └── src/
│       ├── VetSuccess.Api/              # Web API Layer
│       │   ├── Controllers/             # REST API endpoints
│       │   ├── Middleware/              # Exception handling, auth filters
│       │   └── Converters/              # JSON converters
│       │
│       ├── VetSuccess.Application/      # Application Layer
│       │   ├── Services/                # Business logic
│       │   ├── DTOs/                    # Data transfer objects
│       │   ├── Interfaces/              # Service contracts
│       │   ├── Validators/              # FluentValidation rules
│       │   └── Mappings/                # AutoMapper profiles
│       │
│       ├── VetSuccess.Domain/           # Domain Layer
│       │   ├── Entities/                # Domain models
│       │   └── Interfaces/              # Repository contracts
│       │
│       ├── VetSuccess.Infrastructure/   # Infrastructure Layer
│       │   ├── Data/                    # EF Core DbContext & configs
│       │   ├── Repositories/            # Data access implementations
│       │   ├── Services/                # External service integrations
│       │   ├── Jobs/                    # Hangfire background jobs
│       │   └── Configuration/           # Options classes
│       │
│       └── VetSuccess.Shared/           # Shared Layer
│           ├── Constants/               # Application constants
│           ├── Exceptions/              # Custom exceptions
│           ├── Extensions/              # Extension methods
│           └── Utilities/               # Helper utilities
```

## 🛠️ Tech Stack

### Core Framework

- **.NET 8.0** - Latest LTS version
- **ASP.NET Core** - Web API framework
- **C# 12** - Latest language features

### Data Access

- **Entity Framework Core 8.0** - ORM
- **SQL Server 2022** - Primary database
- **Dapper** (optional) - High-performance queries

### Caching & Background Jobs

- **Redis** - Distributed caching
- **Hangfire** - Background job processing

### Authentication & Security

- **ASP.NET Core Identity** - User management
- **JWT Bearer** - Token-based authentication
- **BCrypt** - Password hashing

### External Services

- **Dialpad API** - SMS communication
- **SendGrid** - Email delivery
- **Azure Blob Storage** - File storage
- **Sentry** - Error tracking

### Validation & Mapping

- **FluentValidation** - Request validation
- **AutoMapper** - Object mapping

### Resilience & HTTP

- **Polly** - Retry policies and circuit breakers
- **HttpClientFactory** - Managed HTTP clients

### Logging & Monitoring

- **Serilog** - Structured logging
- **Health Checks** - Application health monitoring

### API Documentation

- **Swagger/OpenAPI** - Interactive API documentation

### Testing

- **xUnit** - Unit testing framework
- **Moq** - Mocking framework
- **FluentAssertions** - Assertion library

## 📦 Prerequisites

Before you begin, ensure you have the following installed:

- **.NET 8.0 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Docker Desktop** - [Download](https://www.docker.com/products/docker-desktop)
- **Git** - [Download](https://git-scm.com/downloads)
- **Visual Studio 2022** or **VS Code** (optional but recommended)

### Verify Installation

```bash
# Check .NET version
dotnet --version
# Should output: 8.0.x

# Check Docker
docker --version
docker-compose --version
```

## 🚀 Getting Started

### 1. Clone the Repository

```bash
git clone <repository-url>
cd backend-dotnet
```

### 2. Start Infrastructure Services

Start SQL Server and Redis using Docker Compose:

```bash
docker-compose -f docker-compose.sqlserver.yml up -d
```

This will start:

- **SQL Server 2022** on port `1433`
- **Redis** on port `6379`

Verify containers are running:

```bash
docker ps
```

### 3. Restore NuGet Packages

```bash
dotnet restore
```

Or use the PowerShell script:

```powershell
.\setup-packages.ps1
```

### 4. Update Configuration

Edit `src/VetSuccess.Api/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=VetSuccess;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True"
  },
  "Jwt": {
    "SecretKey": "your-secret-key-at-least-32-characters-long-for-security"
  }
}
```

### 5. Apply Database Migrations

```bash
cd src/VetSuccess.Api
dotnet ef database update
```

Or let the application auto-migrate on startup (Development mode only).

### 6. Run the Application

```bash
dotnet run --project src/VetSuccess.Api
```

The API will be available at:

- **HTTP**: `http://localhost:5000`
- **HTTPS**: `https://localhost:5001`
- **Swagger UI**: `https://localhost:5001/swagger`
- **Hangfire Dashboard**: `https://localhost:5001/hangfire`

## 📁 Project Structure

### Domain Entities

| Entity             | Description                 | Primary Key        |
| ------------------ | --------------------------- | ------------------ |
| `Client`           | Client information          | CLIENT_ODU_ID      |
| `Patient`          | Pet/patient records         | PATIENT_ODU_ID     |
| `Practice`         | Veterinary practice details | PRACTICE_ODU_ID    |
| `User`             | System users                | id (Guid)          |
| `SMSHistory`       | SMS communication logs      | uuid               |
| `Outcome`          | Call outcome records        | uuid               |
| `Appointment`      | Appointment records         | APPOINTMENT_ODU_ID |
| `Reminder`         | Reminder records            | REMINDER_ODU_ID    |
| `PracticeSettings` | Practice-specific settings  | uuid               |

### API Endpoints

#### Authentication

- `POST /api/v1/auth/login` - User login
- `POST /api/v1/auth/refresh` - Refresh access token
- `POST /api/v1/auth/logout` - User logout

#### Clients

- `GET /api/v1/call-center/clients` - List clients
- `GET /api/v1/call-center/clients/{id}` - Get client details
- `PUT /api/v1/call-center/clients/{id}` - Update client

#### Practices

- `GET /api/v1/call-center/practices` - List practices
- `GET /api/v1/call-center/practices/{id}` - Get practice details
- `PUT /api/v1/call-center/practices/{id}/settings` - Update settings

#### Outcomes

- `POST /api/v1/call-center/outcomes` - Create outcome
- `GET /api/v1/call-center/outcomes` - List outcomes

#### SMS History

- `GET /api/v1/call-center/contacted-clients` - Get contacted clients

## ⚙️ Configuration

### Environment Variables

You can override settings using environment variables:

```bash
# Database
ConnectionStrings__DefaultConnection="Server=..."

# JWT
Jwt__SecretKey="your-secret-key"
Jwt__AccessTokenLifetime=3600

# Redis
Redis__ConnectionString="localhost:6379"

# Dialpad
Dialpad__ApiToken="your-api-token"
Dialpad__SendSms=true

# SendGrid
SendGrid__ApiKey="your-api-key"
SendGrid__DefaultFromEmail="noreply@vetsuccess.com"

# Azure Storage
AzureStorage__AccountName="your-account"
AzureStorage__SasToken="your-sas-token"

# Sentry
Sentry__Dsn="your-sentry-dsn"
Sentry__Enabled=true
```

### appsettings.json Structure

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "SQL Server connection string"
  },
  "Jwt": {
    "SecretKey": "Minimum 32 characters",
    "Issuer": "VetSuccess",
    "Audience": "VetSuccessApi",
    "AccessTokenLifetime": 3600,
    "RefreshTokenLifetime": 604800
  },
  "Redis": {
    "ConnectionString": "localhost:6379"
  },
  "Dialpad": {
    "ApiToken": "Dialpad API token",
    "SendSms": false
  },
  "AzureStorage": {
    "AccountName": "Storage account name",
    "SasToken": "SAS token",
    "ContainerName": "Container name",
    "PathPrefix": "daily_notification"
  },
  "SendGrid": {
    "ApiKey": "SendGrid API key",
    "DefaultFromEmail": "sender@example.com",
    "UseDebugEmail": true,
    "DebugRecipient": "debug@example.com"
  },
  "Sentry": {
    "Dsn": "Sentry DSN",
    "Enabled": false
  },
  "Cors": {
    "AllowedOrigins": ["http://localhost:3000"]
  }
}
```

## 🏃 Running the Application

### Development Mode

```bash
# Run with hot reload
dotnet watch run --project src/VetSuccess.Api

# Run specific environment
dotnet run --project src/VetSuccess.Api --environment Development
```

### Production Mode

```bash
# Build for production
dotnet build -c Release

# Run production build
dotnet run --project src/VetSuccess.Api --environment Production
```

### Using Docker

```bash
# Build Docker image
docker build -t vetsuccess-api .

# Run container
docker run -p 5000:8080 \
  -e ConnectionStrings__DefaultConnection="Server=host.docker.internal;..." \
  vetsuccess-api
```

## 📚 API Documentation

### Swagger UI

Access interactive API documentation at:

- **Development**: `https://localhost:5001/swagger`
- **Production**: `https://your-domain.com/swagger` (if enabled)

### Authentication

Most endpoints require JWT authentication. Include the token in the Authorization header:

```http
Authorization: Bearer <your-jwt-token>
```

### Example Request

```bash
# Login
curl -X POST https://localhost:5001/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "password": "password123"
  }'

# Use token
curl -X GET https://localhost:5001/api/v1/call-center/clients \
  -H "Authorization: Bearer <token>"
```

## 🗄️ Database

### Migrations

```bash
# Create new migration
dotnet ef migrations add MigrationName --project src/VetSuccess.Infrastructure --startup-project src/VetSuccess.Api

# Apply migrations
dotnet ef database update --project src/VetSuccess.Infrastructure --startup-project src/VetSuccess.Api

# Rollback migration
dotnet ef database update PreviousMigrationName --project src/VetSuccess.Infrastructure --startup-project src/VetSuccess.Api

# Remove last migration
dotnet ef migrations remove --project src/VetSuccess.Infrastructure --startup-project src/VetSuccess.Api
```

### SQL Scripts

Manual SQL migrations are located in:

```
src/VetSuccess.Infrastructure/Data/Migrations/
├── 001_AddSmsAndEmailEventTables.sql
├── 002_SeedSmsTemplates.sql
└── apply_all_migrations.sql
```

## ⏰ Background Jobs

### Hangfire Dashboard

Access at: `https://localhost:5001/hangfire`

### Configured Jobs

1. **SMS Aggregation Job**

   - Schedule: Daily at 8:00 AM UTC
   - Purpose: Aggregate SMS statistics

2. **Daily Email Updates Job**

   - Schedule: Daily at 9:00 AM UTC
   - Purpose: Send daily email reports

3. **SMS Sending Job**
   - Schedule: On-demand
   - Purpose: Send queued SMS messages

### Manual Job Execution

```csharp
// Enqueue immediate job
BackgroundJob.Enqueue<ISmsSendingJob>(job => job.ExecuteAsync(CancellationToken.None));

// Schedule delayed job
BackgroundJob.Schedule<ISmsSendingJob>(
    job => job.ExecuteAsync(CancellationToken.None),
    TimeSpan.FromMinutes(30));
```

## 🧪 Testing

### Run All Tests

```bash
dotnet test
```

### Run Specific Test Project

```bash
dotnet test tests/VetSuccess.UnitTests
dotnet test tests/VetSuccess.IntegrationTests
```

### Run with Coverage

```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

## 🚢 Deployment

### Azure App Service

1. Create App Service and SQL Database
2. Configure connection strings in Azure Portal
3. Deploy using:

```bash
# Using Azure CLI
az webapp up --name vetsuccess-api --resource-group vetsuccess-rg

# Using Visual Studio
# Right-click project → Publish → Azure
```

### Docker Deployment

```bash
# Build and push to registry
docker build -t your-registry/vetsuccess-api:latest .
docker push your-registry/vetsuccess-api:latest

# Deploy to container service
docker run -d -p 80:8080 your-registry/vetsuccess-api:latest
```

### Environment-Specific Settings

- Use **Azure Key Vault** for secrets
- Configure **Application Insights** for monitoring
- Set up **Azure Redis Cache** for production caching
- Enable **Auto-scaling** based on load

## 🔍 Health Checks

Monitor application health at:

- `GET /health` - Overall health status

Checks include:

- Database connectivity
- Redis connectivity
- External service availability

## 📊 Monitoring & Logging

### Serilog

Logs are written to:

- **Console** (Development)
- **File** (Production)
- **Sentry** (Errors only)

### Log Levels

- `Information` - General application flow
- `Warning` - Unexpected but handled situations
- `Error` - Errors and exceptions
- `Fatal` - Critical failures

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Code Style

- Follow C# coding conventions
- Use meaningful variable names
- Add XML documentation for public APIs
- Write unit tests for new features

## 📄 License

This project is proprietary and confidential.

## 📞 Support

For issues and questions:

- Create an issue in the repository
- Contact the development team
- Check the documentation in `.kiro/specs/`

## 🎯 Roadmap

- [x] Domain entities and DbContext
- [x] Repository pattern
- [x] Application services
- [x] API controllers
- [x] Authentication & authorization
- [x] Background jobs
- [ ] Unit tests (in progress)
- [ ] Integration tests (in progress)
- [ ] Performance optimization
- [ ] API versioning
- [ ] Rate limiting
- [ ] GraphQL support (future)

---

**Built with ❤️ using ASP.NET Core 8.0**
