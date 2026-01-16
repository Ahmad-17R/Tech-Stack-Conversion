using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Polly;
using Serilog;
using Serilog.Events;
using StackExchange.Redis;
using VetSuccess.Api.Middleware;
using VetSuccess.Application.Interfaces;
using VetSuccess.Application.Mappings;
using VetSuccess.Application.Services;
using VetSuccess.Domain.Entities;
using VetSuccess.Domain.Interfaces;
using VetSuccess.Infrastructure.Configuration;
using VetSuccess.Infrastructure.Data;
using VetSuccess.Infrastructure.Repositories;
using VetSuccess.Infrastructure.Services;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "VetSuccess")
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .CreateLogger();

try
{
    Log.Information("Starting VetSuccess API");

var builder = WebApplication.CreateBuilder(args);

// Use Serilog for logging
builder.Host.UseSerilog();

// Configure Sentry (conditional based on configuration)
var sentryDsn = builder.Configuration["Sentry:Dsn"];
var sentryEnabled = builder.Configuration.GetValue<bool>("Sentry:Enabled");

if (sentryEnabled && !string.IsNullOrEmpty(sentryDsn))
{
    builder.WebHost.UseSentry(options =>
    {
        options.Dsn = sentryDsn;
        options.Environment = builder.Environment.EnvironmentName;
        options.TracesSampleRate = builder.Configuration.GetValue<double>("Sentry:TracesSampleRate", 0.1);
        options.AttachStacktrace = true;
        options.SendDefaultPii = false;
        options.MaxBreadcrumbs = 50;
        options.Debug = builder.Environment.IsDevelopment();
    });
    
    Log.Information("Sentry error tracking enabled");
}

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.Converters.Add(new VetSuccess.Api.Converters.DateTimeConverter());
        options.JsonSerializerOptions.Converters.Add(new VetSuccess.Api.Converters.NullableDateTimeConverter());
    });

// Configure FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<VetSuccess.Application.Validators.ClientUpdateDtoValidator>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "VetSuccess API", 
        Version = "v1",
        Description = "VetSuccess Call Center API"
    });
    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configure Database
builder.Services.AddDbContext<VetSuccessDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Redis
var redisConnectionString = builder.Configuration.GetSection("Redis:ConnectionString").Value 
    ?? "localhost:6379";
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = ConfigurationOptions.Parse(redisConnectionString);
    configuration.AbortOnConnectFail = false;
    configuration.ConnectRetry = 3;
    configuration.ConnectTimeout = 5000;
    configuration.SyncTimeout = 5000;
    return ConnectionMultiplexer.Connect(configuration);
});
builder.Services.AddSingleton<ICacheService, RedisCacheService>();

// Configure ASP.NET Core Identity
builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;

    // User settings
    options.User.RequireUniqueEmail = true;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
})
.AddEntityFrameworkStores<VetSuccessDbContext>()
.AddDefaultTokenProviders();

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is not configured");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Configure Health Checks
builder.Services.AddHealthChecks()
    .AddSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection") ?? "",
        name: "database",
        tags: new[] { "db", "sql", "sqlserver" })
    .AddRedis(
        builder.Configuration["Redis:ConnectionString"] ?? "localhost:6379",
        name: "redis",
        tags: new[] { "cache", "redis" });

// Configure Hangfire
builder.Services.AddHangfire(config =>
{
    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddHangfireServer(options =>
{
    options.WorkerCount = Environment.ProcessorCount * 2;
    options.ServerName = $"{Environment.MachineName}:{Guid.NewGuid()}";
});

// Register Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IPracticeRepository, PracticeRepository>();
builder.Services.AddScoped<ISMSHistoryRepository, SMSHistoryRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register Application Services
builder.Services.AddScoped<IAuthenticationService, JwtTokenService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IPracticeService, PracticeService>();
builder.Services.AddScoped<IOutcomeService, OutcomeService>();
builder.Services.AddScoped<ISMSHistoryService, SMSHistoryService>();
builder.Services.AddScoped<IOutcomeSideEffectsService, OutcomeSideEffectsService>();

// Register Admin Services
builder.Services.AddScoped<IUserAdminService, UserAdminService>();
builder.Services.AddScoped<IOutcomeAdminService, OutcomeAdminService>();
builder.Services.AddScoped<IQuestionAdminService, QuestionAdminService>();
builder.Services.AddScoped<IAnswerAdminService, AnswerAdminService>();
builder.Services.AddScoped<IPracticeAdminService, PracticeAdminService>();
builder.Services.AddScoped<IPracticeSettingsAdminService, PracticeSettingsAdminService>();
builder.Services.AddScoped<ISMSTemplateAdminService, SMSTemplateAdminService>();

// Register Background Jobs
builder.Services.AddScoped<ISmsAggregationJob, VetSuccess.Infrastructure.Jobs.SmsAggregationJob>();
builder.Services.AddScoped<ISmsSendingJob, VetSuccess.Infrastructure.Jobs.SmsSendingJob>();
builder.Services.AddScoped<IDailyEmailUpdatesJob, VetSuccess.Infrastructure.Jobs.DailyEmailUpdatesJob>();

// Configure and register external services
builder.Services.Configure<DialpadOptions>(builder.Configuration.GetSection(DialpadOptions.SectionName));
builder.Services.Configure<AzureStorageOptions>(builder.Configuration.GetSection(AzureStorageOptions.SectionName));
builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection(EmailOptions.SectionName));

// Register Dialpad service with HttpClient and Polly retry policy
builder.Services.AddHttpClient<IDialpadService, DialpadService>()
    .AddTransientHttpErrorPolicy(policy => 
        policy.WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));

// Register Azure Blob Storage service
builder.Services.AddSingleton<IBlobStorageService, AzureBlobStorageService>();

// Register SendGrid email service
builder.Services.AddSingleton<IEmailService, SendGridEmailService>();

// Register AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile), typeof(VetSuccess.Application.Mappings.AdminMappingProfile));

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() 
            ?? new[] { "http://localhost:3000" };
        
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure recurring Hangfire jobs
var recurringJobManager = app.Services.GetRequiredService<IRecurringJobManager>();

recurringJobManager.AddOrUpdate<ISmsAggregationJob>(
    "sms-aggregation",
    job => job.ExecuteAsync(CancellationToken.None),
    Cron.Daily(8, 0), // 8 AM UTC
    new RecurringJobOptions
    {
        TimeZone = TimeZoneInfo.Utc
    });

recurringJobManager.AddOrUpdate<IDailyEmailUpdatesJob>(
    "daily-email-updates",
    job => job.ExecuteAsync(CancellationToken.None),
    Cron.Daily(9, 0), // 9 AM UTC
    new RecurringJobOptions
    {
        TimeZone = TimeZoneInfo.Utc
    });

Log.Information("Hangfire recurring jobs configured");

// Apply migrations automatically in development
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<VetSuccessDbContext>();
    
    try
    {
        await dbContext.Database.MigrateAsync();
        Log.Information("Database migrations applied successfully");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Error applying database migrations");
    }
}

// Configure the HTTP request pipeline
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "VetSuccess API v1");
    });
    
    // Enable Hangfire Dashboard in development
    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = new[] { new HangfireAuthorizationFilter() }
    });
}
else
{
    // In production, require authentication for Hangfire Dashboard
    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = new[] { new HangfireAuthorizationFilter() }
    });
}

app.UseHttpsRedirection();

app.UseStaticFiles(); // Enable static file serving from wwwroot

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");

app.MapControllers();

app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
