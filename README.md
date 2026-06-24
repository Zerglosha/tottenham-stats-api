# Tottenham Stats API

Backend API for manually tracking Tottenham Hotspur football statistics.

The project is built as a portfolio-focused ASP.NET Core application. Its main goal is to show a clean, explainable backend API that can evolve from CRUD operations into useful football statistics, dashboard endpoints, authentication, tests, and deployment.

Current release: `v0.3.0`

## What The API Does

The API currently supports managing and reading:

- clubs;
- players;
- matches;
- competition standings.

The current version focuses on API quality:

- CRUD endpoints for the main football entities;
- request and query validation with DataAnnotations;
- reusable Minimal API validation filter;
- consistent validation errors;
- structured `404` responses with ProblemDetails;
- Swagger/OpenAPI summaries and response metadata;
- filtering, search, and stable sorting for list endpoints;
- read-only EF Core queries with `AsNoTracking()`;
- `CancellationToken` support for GET endpoints.

## Tech Stack

- C#;
- ASP.NET Core 8;
- Minimal APIs;
- Entity Framework Core;
- PostgreSQL;
- Npgsql;
- Swagger/OpenAPI.

## API Areas

| Area | Route |
| --- | --- |
| Players | `/api/players` |
| Clubs | `/api/clubs` |
| Matches | `/api/matches` |
| Competition standings | `/api/competition-standings` |

Examples:

```http
GET /api/players?search=son&isInjured=false
GET /api/clubs?season=2025/26
GET /api/matches?competition=Premier League&isHome=true
GET /api/competition-standings?competition=Premier League
```

## Project Structure

```text
src/TottenhamStatsAPI/
├── Data/                 EF Core DbContext
├── DTOs/                 request, response, and query DTOs
├── Endpoints/            Minimal API endpoint groups
├── Filters/              endpoint filters, including validation
├── Helpers/              shared API response helpers
├── Migrations/           EF Core migrations
├── Models/               domain/database entities
└── Program.cs            application setup and endpoint registration
```

The project intentionally keeps the architecture simple. There is no service or repository layer yet because the current behavior is mostly CRUD and read-only querying. More structure will be added later only when dashboard/statistics logic makes it useful.

## Running Locally

Requirements:

- .NET 8 SDK;
- PostgreSQL;
- EF Core CLI tools if you want to apply migrations from the terminal.

Set the connection string with user secrets:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=tottenham_stats;Username=postgres;Password=your_password" --project src/TottenhamStatsAPI
```

Apply migrations:

```bash
dotnet ef database update --project src/TottenhamStatsAPI
```

Run the API:

```bash
dotnet run --project src/TottenhamStatsAPI
```

Swagger UI is available in development mode:

```text
http://localhost:5227/swagger
```

## Development Status

Completed releases:

- `v0.1.0` - database schema and EF Core setup;
- `v0.2.0` - CRUD API;
- `v0.3.0` - API quality, validation, errors, OpenAPI metadata, filtering/search.

Current focus:

- `v0.4.0` - dashboard and statistics endpoints.

See [ROADMAP.md](ROADMAP.md) for the planned version-by-version development path and [CHANGELOG.md](CHANGELOG.md) for released changes.

## Why This Project Exists

This repository is a learning and portfolio project. The emphasis is not only on adding features, but also on being able to explain:

- why the API is structured this way;
- how ASP.NET Core Minimal APIs handle routing, binding, filters, and responses;
- how EF Core queries are built and executed;
- how validation and error responses are represented for API clients;
- how the project can grow without adding unnecessary architecture too early.
