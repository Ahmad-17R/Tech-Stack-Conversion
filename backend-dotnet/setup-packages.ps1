# Install packages for Infrastructure
Write-Host "Installing Infrastructure packages..."
cd src/VetSuccess.Infrastructure
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.0
dotnet add package StackExchange.Redis --version 2.7.10
dotnet add package Hangfire.Core --version 1.8.9
dotnet add package Hangfire.PostgreSql --version 1.20.8
dotnet add package Azure.Storage.Blobs --version 12.19.1
dotnet add package SendGrid --version 9.29.3
dotnet add package Serilog.AspNetCore --version 8.0.0
dotnet add package Serilog.Sinks.Console --version 5.0.1
dotnet add package Sentry.AspNetCore --version 4.0.2
dotnet add package Microsoft.Extensions.Http.Polly --version 8.0.0
cd ../..

# Install packages for Application
Write-Host "Installing Application packages..."
cd src/VetSuccess.Application
dotnet add package AutoMapper --version 13.0.1
dotnet add package FluentValidation --version 11.9.0
dotnet add package FluentValidation.DependencyInjectionExtensions --version 11.9.0
cd ../..

# Install packages for Api
Write-Host "Installing Api packages..."
cd src/VetSuccess.Api
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 8.0.0
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore --version 8.0.0
dotnet add package Swashbuckle.AspNetCore --version 6.5.0
dotnet add package Hangfire.AspNetCore --version 1.8.9
cd ../..

Write-Host "Package installation complete!"
