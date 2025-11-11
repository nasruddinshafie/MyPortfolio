# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a .NET 9 portfolio application built using Clean Architecture principles with the following project structure:

- **WebApi** - API layer (ASP.NET Core Web API)
- **Application** - Application logic and use cases 
- **Domain** - Core business entities and domain logic
- **Infrastructure** - External concerns (data access, external APIs, etc.)

## Build and Run Commands

### Building the Solution
```bash
dotnet build
```

### Running the API
```bash
# From the WebApi directory
cd WebApi
dotnet run

# Or from solution root
dotnet run --project WebApi
```

The API runs on:
- HTTP: http://localhost:5240
- HTTPS: https://localhost:7278

### Testing
```bash
# Run all tests
dotnet test

# Run tests for specific project
dotnet test <ProjectName>
```

### Restore Dependencies
```bash
dotnet restore
```

## Architecture Notes

### Clean Architecture Layers
- **Domain**: Contains entities, value objects, and domain logic (currently has placeholder folders)
- **Application**: Contains application services, DTOs, and business workflows
- **Infrastructure**: Contains implementations of external dependencies
- **WebApi**: Contains controllers, middleware, and API configuration

### Key Configuration
- Target Framework: .NET 9
- Nullable reference types enabled
- Implicit usings enabled
- Default API runs without Swagger/OpenAPI (minimal setup)

### Project Dependencies
The solution follows Clean Architecture dependency rules:
- Domain has no dependencies
- Application depends only on Domain  
- Infrastructure depends on Application and Domain
- WebApi depends on Application and can reference Infrastructure for DI setup

## Development Notes

### Entry Point
- Main API entry point: `WebApi/Program.cs`
- Basic ASP.NET Core setup with controllers and HTTPS redirection

### Current State
- Template-based project with minimal controllers (WeatherForecast example)
- Ready for portfolio-specific feature development
- Clean slate for implementing portfolio functionality