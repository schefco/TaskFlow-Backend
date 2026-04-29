# TaskFlow Backend

TaskFlow is a project and task management platform built with a clean, modular architecture.  
This repository contains the backend API built with .NET 8, Entity Framework Core, and PostgreSQL.

## Features

- JWT authentication with secure password hashing
- Role-based authorization (Owner and User)
- First-time password reset workflow
- Project and task management endpoints
- User administration (Owner only)
- Pending user approval system
- Clean Architecture separation of concerns
- Database migrations and EF Core integration

## Tech Stack

- .NET 8 Web API
- Entity Framework Core
- PostgreSQL (Neon or any Postgres provider)
- FluentValidation
- Clean Architecture (Domain, Application, Infrastructure, API)
- JWT Authentication
- ASP.NET Identity-style patterns

## Project Structure

src/
Domain/            Entities, enums, core logic
Application/       CQRS handlers, validation, interfaces
Infrastructure/    EF Core, database, repositories
API/               Controllers, middleware, DI setup

Code

## Environment Variables

Create an `appsettings.Production.json` or use environment variables:

DATABASE_URL=your-postgres-connection-string
JWT__Key=your-secret-key
JWT__Issuer=TaskFlow
JWT__Audience=TaskFlowUsers

Code

## Running the Project

Restore dependencies:

dotnet restore

Code

Apply migrations:

dotnet ef database update

Code

Run the API:

dotnet run --project src/API

Code

## Authentication and Authorization

- Users authenticate via email and password.
- JWT tokens include userId, email, and role claims.
- Owner-only endpoints are protected with authorization policies.
- First-time login requires a password reset using a temporary token.

## Key Endpoints

### Authentication
- `POST /auth/login`
- `POST /auth/first-time-password`
- `GET /auth/me`

### Users (Owner Only)
- `GET /users`
- `GET /users/{id}`
- `POST /users/{id}/reset-password`
- `DELETE /users/{id}`

### Pending Users (Owner Only)
- `GET /pending-users`
- `POST /pending-users/{id}/approve`
- `POST /pending-users/{id}/deny`

### Projects
- `GET /projects`
- `POST /projects`
- `GET /projects/{id}`

## Clean Architecture Overview

- **Domain** contains core business rules.
- **Application** contains use cases and validation.
- **Infrastructure** handles EF Core, database, and external services.
- **API** exposes endpoints and configures middleware.

This structure ensures testability, maintainability, and clear separation of concerns.

## License

This project is open source and available for review, learning, and extension.
