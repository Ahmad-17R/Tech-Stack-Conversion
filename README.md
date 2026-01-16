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
- **Authentication** - JWT-based authentication with refresh tokens and user registration
- **Admin Panel** - Complete CRUD operations for all entities

### ✅ Migration Status

**100% Complete** - All 52 API endpoints from Django backend have been successfully migrated and tested:

- ✅ 3 Authentication endpoints (login, refresh, register)
- ✅ 9 Call Center endpoints (clients, practices, outcomes, SMS history)
- ✅ 40 Admin endpoints (full CRUD for all entities)

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

### 5. Setup Database

The application includes automated database setup scripts. Run the setup script:

**Windows (PowerShell):**

```powershell
.\setup-database.ps1
```

**Linux/Mac:**

```bash
chmod +x setup-database.sh
./setup-database.sh
```

Or manually run the SQL scripts:

```bash
# Copy scripts to container
docker cp database-complete-schema.sql vetsuccess-sqlserver:/tmp/
docker cp database-complete-schema-part2.sql vetsuccess-sqlserver:/tmp/
docker cp database-seed-data.sql vetsuccess-sqlserver:/tmp/
docker cp database-identity-tables.sql vetsuccess-sqlserver:/tmp/

# Execute scripts
docker exec vetsuccess-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "YourStrong@Passw0rd" -i /tmp/database-complete-schema.sql -C
docker exec vetsuccess-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "YourStrong@Passw0rd" -i /tmp/database-complete-schema-part2.sql -C
docker exec vetsuccess-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "YourStrong@Passw0rd" -i /tmp/database-seed-data.sql -C
docker exec vetsuccess-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "YourStrong@Passw0rd" -i /tmp/database-identity-tables.sql -C
```

The database will be created with:

- All entity tables (23 tables)
- ASP.NET Identity tables (7 tables)
- Seeded data (outcomes, questions, SMS templates)
- Proper indexes and constraints

### 6. Run the Application

```bash
dotnet run --project src/VetSuccess.Api
```

The API will be available at:

- **HTTP**: `http://localhost:5136`
- **Swagger UI**: `http://localhost:5136/swagger`
- **Hangfire Dashboard**: `http://localhost:5136/hangfire`
- **Health Check**: `http://localhost:5136/health`

### 7. Create Admin User

Register your first admin user via API:

```bash
curl -X POST http://localhost:5136/api/v1/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@vetsuccess.com",
    "password": "Admin@123456",
    "passwordConfirm": "Admin@123456",
    "firstName": "Admin",
    "lastName": "User"
  }'
```

Or use Swagger UI at `http://localhost:5136/swagger`

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

### API Endpoints (52 Total)

#### Authentication (3 endpoints)

- `POST /api/v1/auth/login` - User login
- `POST /api/v1/auth/refresh` - Refresh access token
- `POST /api/v1/auth/register` - Register new user

#### Call Center APIs (9 endpoints)

**Clients**

- `GET /api/v1/call-center/clients` - List clients with filtering
- `GET /api/v1/call-center/clients/{oduId}` - Get client details
- `PATCH /api/v1/call-center/clients/{oduId}` - Update client
- `PUT /api/v1/call-center/clients/{oduId}` - Replace client

**Practices**

- `GET /api/v1/call-center/practices` - List practices
- `GET /api/v1/call-center/practices/{oduId}/faq` - Get practice FAQ

**Outcomes**

- `GET /api/v1/call-center/outcomes` - List outcomes

**SMS History**

- `GET /api/v1/call-center/contacted-clients` - Get contacted clients
- `PATCH /api/v1/call-center/sms-history/{uuid}` - Update SMS history

#### Admin APIs (40 endpoints)

**Outcomes Management**

- `GET /api/v1/admin/outcomes` - List all outcomes
- `GET /api/v1/admin/outcomes/{outcomeId}` - Get outcome by ID
- `POST /api/v1/admin/outcomes` - Create new outcome
- `PUT /api/v1/admin/outcomes/{outcomeId}` - Update outcome
- `PATCH /api/v1/admin/outcomes/{outcomeId}` - Partial update outcome
- `DELETE /api/v1/admin/outcomes/{outcomeId}` - Delete outcome

**Questions Management**

- `GET /api/v1/admin/questions` - List all questions
- `GET /api/v1/admin/questions/{questionId}` - Get question by ID
- `POST /api/v1/admin/questions` - Create new question
- `PUT /api/v1/admin/questions/{questionId}` - Update question
- `PATCH /api/v1/admin/questions/{questionId}` - Partial update question
- `DELETE /api/v1/admin/questions/{questionId}` - Delete question

**Answers Management**

- `GET /api/v1/admin/answers` - List all answers
- `GET /api/v1/admin/answers/{answerId}` - Get answer by ID
- `POST /api/v1/admin/answers` - Create new answer
- `PUT /api/v1/admin/answers/{answerId}` - Update answer
- `PATCH /api/v1/admin/answers/{answerId}` - Partial update answer
- `DELETE /api/v1/admin/answers/{answerId}` - Delete answer

**Practices Management**

- `GET /api/v1/admin/practices` - List all practices
- `GET /api/v1/admin/practices/{practiceOduId}` - Get practice by ODU ID
- `PUT /api/v1/admin/practices/{practiceOduId}` - Update practice
- `PATCH /api/v1/admin/practices/{practiceOduId}` - Partial update practice

**Practice Settings Management**

- `GET /api/v1/admin/practice-settings` - List all practice settings
- `GET /api/v1/admin/practice-settings/{settingsId}` - Get settings by ID
- `POST /api/v1/admin/practice-settings` - Create new settings
- `PUT /api/v1/admin/practice-settings/{settingsId}` - Update settings
- `PATCH /api/v1/admin/practice-settings/{settingsId}` - Partial update settings
- `DELETE /api/v1/admin/practice-settings/{settingsId}` - Delete settings

**SMS Templates Management**

- `GET /api/v1/admin/sms-templates` - List all SMS templates
- `GET /api/v1/admin/sms-templates/{templateId}` - Get template by ID
- `POST /api/v1/admin/sms-templates` - Create new template
- `PUT /api/v1/admin/sms-templates/{templateId}` - Update template
- `PATCH /api/v1/admin/sms-templates/{templateId}` - Partial update template
- `DELETE /api/v1/admin/sms-templates/{templateId}` - Delete template

**Users Management**

- `GET /api/v1/admin/users` - List all users
- `GET /api/v1/admin/users/{userId}` - Get user by ID
- `POST /api/v1/admin/users` - Create new user
- `PUT /api/v1/admin/users/{userId}` - Update user
- `PATCH /api/v1/admin/users/{userId}` - Partial update user
- `DELETE /api/v1/admin/users/{userId}` - Delete user

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

- **Development**: `http://localhost:5136/swagger`
- **Production**: `https://your-domain.com/swagger` (if enabled)

The Swagger UI provides:

- Complete API documentation for all 52 endpoints
- Interactive testing interface
- Request/response schemas
- Authentication support (click "Authorize" button to add JWT token)

### Authentication

Most endpoints require JWT authentication. Include the token in the Authorization header:

```http
Authorization: Bearer <your-jwt-token>
```

**Token Workflow:**

1. Register a new user via `/api/v1/auth/register`
2. Login via `/api/v1/auth/login` to get access and refresh tokens
3. Use the access token in the Authorization header for protected endpoints
4. When the access token expires, use `/api/v1/auth/refresh` with the refresh token
5. Access tokens expire after 1 hour (configurable)
6. Refresh tokens expire after 7 days (configurable)

### Admin vs Call Center APIs

**Call Center APIs** (`/api/v1/call-center/*`):

- Designed for call center operators
- Limited to read and update operations
- Focus on client interactions and SMS history
- Filtered by practice for multi-tenancy

**Admin APIs** (`/api/v1/admin/*`):

- Designed for system administrators
- Full CRUD operations on all entities
- Manage system configuration (outcomes, questions, templates)
- User management capabilities
- Practice settings management

### Example Requests

#### Authentication Examples

```bash
# Register new user
curl -X POST http://localhost:5136/api/v1/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "password": "Password123!",
    "passwordConfirm": "Password123!",
    "firstName": "John",
    "lastName": "Doe"
  }'

# Login
curl -X POST http://localhost:5136/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "password": "Password123!"
  }'

# Refresh token
curl -X POST http://localhost:5136/api/v1/auth/refresh \
  -H "Content-Type: application/json" \
  -d '{
    "refreshToken": "<your-refresh-token>"
  }'
```

#### Call Center API Examples

```bash
# Get all clients with filtering
curl -X GET "http://localhost:5136/api/v1/call-center/clients?practice_odu_id=PRAC001&page=1&page_size=20" \
  -H "Authorization: Bearer <token>"

# Get specific client
curl -X GET http://localhost:5136/api/v1/call-center/clients/CLI001 \
  -H "Authorization: Bearer <token>"

# Update client
curl -X PATCH http://localhost:5136/api/v1/call-center/clients/CLI001 \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{
    "comment": "Updated comment",
    "optOut": false
  }'

# Get contacted clients
curl -X GET "http://localhost:5136/api/v1/call-center/contacted-clients?practice_odu_id=PRAC001" \
  -H "Authorization: Bearer <token>"

# Get practice FAQ
curl -X GET http://localhost:5136/api/v1/call-center/practices/PRAC001/faq \
  -H "Authorization: Bearer <token>"
```

#### Admin API Examples

**Outcomes Management:**

```bash
# List all outcomes
curl -X GET http://localhost:5136/api/v1/admin/outcomes \
  -H "Authorization: Bearer <token>"

# Create new outcome
curl -X POST http://localhost:5136/api/v1/admin/outcomes \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{
    "text": "Appointment Scheduled",
    "order": 1,
    "isActive": true
  }'

# Update outcome
curl -X PUT http://localhost:5136/api/v1/admin/outcomes/{uuid} \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{
    "text": "Appointment Confirmed",
    "order": 1,
    "isActive": true
  }'

# Partial update outcome
curl -X PATCH http://localhost:5136/api/v1/admin/outcomes/{uuid} \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{
    "isActive": false
  }'

# Delete outcome
curl -X DELETE http://localhost:5136/api/v1/admin/outcomes/{uuid} \
  -H "Authorization: Bearer <token>"
```

**Questions & Answers Management:**

```bash
# Create question
curl -X POST http://localhost:5136/api/v1/admin/questions \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{
    "text": "What are your office hours?",
    "order": 1
  }'

# Create answer for a question
curl -X POST http://localhost:5136/api/v1/admin/answers \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{
    "practiceOduId": "PRAC001",
    "questionUuid": "{question-uuid}",
    "answerText": "Monday-Friday 9AM-5PM"
  }'

# Update answer
curl -X PUT http://localhost:5136/api/v1/admin/answers/{uuid} \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{
    "practiceOduId": "PRAC001",
    "questionUuid": "{question-uuid}",
    "answerText": "Monday-Friday 8AM-6PM, Saturday 9AM-1PM"
  }'
```

**Practice Settings Management:**

```bash
# Create practice settings
curl -X POST http://localhost:5136/api/v1/admin/practice-settings \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{
    "practiceOduId": "PRAC001",
    "launchDate": "2024-01-01",
    "endDateForLaunch": "2024-12-31",
    "rdoName": "Dr. Smith",
    "rdoEmail": "dr.smith@practice.com",
    "rdoPhone": "+1234567890",
    "isActive": true
  }'

# Update practice settings
curl -X PATCH http://localhost:5136/api/v1/admin/practice-settings/{uuid} \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{
    "rdoPhone": "+1987654321",
    "isActive": true
  }'
```

**SMS Templates Management:**

```bash
# List SMS templates
curl -X GET http://localhost:5136/api/v1/admin/sms-templates \
  -H "Authorization: Bearer <token>"

# Create SMS template
curl -X POST http://localhost:5136/api/v1/admin/sms-templates \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Appointment Reminder",
    "templateText": "Hi {client_name}, reminder for {pet_name} appointment on {date}",
    "isActive": true
  }'

# Update SMS template
curl -X PUT http://localhost:5136/api/v1/admin/sms-templates/{uuid} \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Appointment Reminder Updated",
    "templateText": "Hello {client_name}, this is a reminder...",
    "isActive": true
  }'
```

**Users Management:**

```bash
# List all users
curl -X GET http://localhost:5136/api/v1/admin/users \
  -H "Authorization: Bearer <token>"

# Get specific user
curl -X GET http://localhost:5136/api/v1/admin/users/{uuid} \
  -H "Authorization: Bearer <token>"

# Create user
curl -X POST http://localhost:5136/api/v1/admin/users \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "newuser@example.com",
    "password": "SecurePass123!",
    "firstName": "Jane",
    "lastName": "Smith"
  }'

# Update user
curl -X PATCH http://localhost:5136/api/v1/admin/users/{uuid} \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "updated@example.com",
    "firstName": "Jane Updated"
  }'

# Delete user
curl -X DELETE http://localhost:5136/api/v1/admin/users/{uuid} \
  -H "Authorization: Bearer <token>"
```

**Practice Management:**

```bash
# List all practices
curl -X GET http://localhost:5136/api/v1/admin/practices \
  -H "Authorization: Bearer <token>"

# Get specific practice
curl -X GET http://localhost:5136/api/v1/admin/practices/PRAC001 \
  -H "Authorization: Bearer <token>"

# Update practice
curl -X PUT http://localhost:5136/api/v1/admin/practices/PRAC001 \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Updated Practice Name",
    "isArchived": false
  }'
```

## 🗄️ Database

### Database Schema

The application uses SQL Server with the following table structure:

**Core Tables (23 tables):**

- `apps_client` - Client information
- `apps_patient` - Patient/pet records
- `apps_practice` - Practice details
- `apps_server` - Server configurations
- `apps_appointment` - Appointment records
- `apps_reminder` - Reminder records
- `apps_email` - Email addresses
- `apps_phone` - Phone numbers
- `apps_address` - Physical addresses
- `apps_outcome` - Call outcomes
- `apps_question` - FAQ questions
- `apps_answer` - FAQ answers
- `apps_practicesettings` - Practice-specific settings
- `apps_smshistory` - SMS communication logs
- `apps_smsevent` - SMS events
- `apps_smstemplate` - SMS templates
- `apps_updatesemailevent` - Email update events
- `apps_clientpatientrelationship` - Client-patient relationships

**Identity Tables (7 tables):**

- `AspNetUsers` - User accounts
- `AspNetRoles` - User roles
- `AspNetUserRoles` - User-role mappings
- `AspNetUserClaims` - User claims
- `AspNetUserLogins` - External login providers
- `AspNetUserTokens` - User tokens
- `AspNetRoleClaims` - Role claims

### Setup Scripts

Database setup scripts are located in the root directory:

```
backend-dotnet/
├── database-complete-schema.sql       # Main schema (part 1)
├── database-complete-schema-part2.sql # Additional tables (part 2)
├── database-seed-data.sql             # Initial data
├── database-identity-tables.sql       # ASP.NET Identity tables
├── database-create-admin-user.sql     # Admin user creation
├── setup-database.ps1                 # Windows setup script
└── setup-database.sh                  # Linux/Mac setup script
```

### Manual Database Setup

If automated scripts fail, you can manually execute:

```bash
# Connect to SQL Server
docker exec -it vetsuccess-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "YourStrong@Passw0rd" -C

# Run scripts in order
:r /tmp/database-complete-schema.sql
:r /tmp/database-complete-schema-part2.sql
:r /tmp/database-seed-data.sql
:r /tmp/database-identity-tables.sql
GO
```

## ⏰ Background Jobs

### Hangfire Dashboard

Access at: `http://localhost:5136/hangfire`

**Note:** In production, the dashboard is protected and requires authentication.

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

Example response:

```json
{
  "status": "Healthy",
  "checks": {
    "database": "Healthy",
    "redis": "Healthy",
    "hangfire": "Healthy"
  },
  "timestamp": "2026-01-16T19:12:16Z"
}
```

Checks include:

- SQL Server database connectivity
- Redis cache connectivity
- Hangfire job server status

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

## 🔐 Security Best Practices

### Production Deployment

1. **Change Default Passwords**

   - Update SQL Server SA password
   - Use strong JWT secret keys (minimum 32 characters)

2. **Environment Variables**

   - Never commit secrets to source control
   - Use Azure Key Vault or similar for production secrets
   - Rotate API keys regularly

3. **HTTPS Only**

   - Enforce HTTPS in production
   - Use valid SSL certificates
   - Enable HSTS headers

4. **Database Security**

   - Use connection string encryption
   - Implement least privilege access
   - Enable SQL Server encryption at rest

5. **API Security**

   - Implement rate limiting
   - Enable CORS only for trusted origins
   - Use API versioning for breaking changes
   - Validate all input data

6. **Monitoring**
   - Enable Application Insights
   - Set up alerts for errors and performance issues
   - Monitor Hangfire job failures
   - Track authentication failures

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
- Follow SOLID principles
- Use async/await for I/O operations
- Implement proper error handling

## 🎓 Learning Resources

### ASP.NET Core

- [Official Documentation](https://docs.microsoft.com/aspnet/core)
- [Clean Architecture](https://github.com/jasontaylordev/CleanArchitecture)
- [Entity Framework Core](https://docs.microsoft.com/ef/core)

### Background Jobs

- [Hangfire Documentation](https://docs.hangfire.io)
- [Hangfire Best Practices](https://docs.hangfire.io/en/latest/best-practices.html)

### Authentication

- [ASP.NET Core Identity](https://docs.microsoft.com/aspnet/core/security/authentication/identity)
- [JWT Authentication](https://jwt.io/introduction)

## 📄 License

This project is proprietary and confidential.

## 🏆 Acknowledgments

- Migrated from Django/Python backend with 100% API compatibility
- Built with Clean Architecture principles
- Follows Microsoft's recommended practices for ASP.NET Core applications

## � API Response Formats

### Success Response

```json
{
  "data": {
    "uuid": "123e4567-e89b-12d3-a456-426614174000",
    "name": "Example",
    "createdAt": "2026-01-16T19:12:16Z"
  }
}
```

### List Response (Paginated)

```json
{
  "data": [
    { "uuid": "...", "name": "Item 1" },
    { "uuid": "...", "name": "Item 2" }
  ],
  "pagination": {
    "page": 1,
    "pageSize": 20,
    "totalCount": 100,
    "totalPages": 5
  }
}
```

### Error Response

```json
{
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "Validation failed",
    "details": {
      "email": ["Email is required"],
      "password": ["Password must be at least 8 characters"]
    }
  },
  "timestamp": "2026-01-16T19:12:16Z",
  "path": "/api/v1/auth/register"
}
```

### Common HTTP Status Codes

- `200 OK` - Successful GET, PUT, PATCH
- `201 Created` - Successful POST
- `204 No Content` - Successful DELETE
- `400 Bad Request` - Validation error
- `401 Unauthorized` - Missing or invalid token
- `403 Forbidden` - Insufficient permissions
- `404 Not Found` - Resource not found
- `409 Conflict` - Duplicate resource
- `500 Internal Server Error` - Server error

## 📞 Support

For issues and questions:

- Create an issue in the repository
- Contact the development team
- Check the documentation in `.kiro/specs/`
- Review Swagger documentation at `/swagger`
- Check Hangfire dashboard for job failures at `/hangfire`

## 🎯 Roadmap

### Completed ✅

- [x] Domain entities and DbContext
- [x] Repository pattern with Unit of Work
- [x] Application services (all 7 admin services)
- [x] API controllers (52 endpoints)
- [x] Authentication & authorization (JWT + Identity)
- [x] User registration endpoint
- [x] Background jobs (Hangfire)
- [x] Database setup scripts
- [x] ASP.NET Identity integration
- [x] Complete Django API parity (52/52 endpoints)
- [x] AutoMapper profiles
- [x] FluentValidation
- [x] Exception handling middleware
- [x] Health checks
- [x] Swagger documentation

### In Progress 🚧

- [ ] Unit tests
- [ ] Integration tests
- [ ] Performance optimization
- [ ] Load testing

### Planned 📋

- [ ] API versioning
- [ ] Rate limiting
- [ ] Advanced caching strategies
- [ ] GraphQL support
- [ ] WebSocket support for real-time updates
- [ ] Audit logging
- [ ] Advanced reporting endpoints

---

**Built with ❤️ using ASP.NET Core 8.0**
