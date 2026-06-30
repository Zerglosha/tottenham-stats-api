# Tottenham Stats API

Backend API for manually tracking Tottenham Hotspur football statistics.

The project is built as a portfolio-focused ASP.NET Core application. Its main goal is to show a clean, explainable backend API that can grow from CRUD operations into dashboard data, tests, authentication, deployment, and observability.

Current release: `v0.5.0`

## What The API Does

The API currently supports:

- managing clubs, players, matches, and competition standings;
- validating request and query data;
- returning consistent validation and not-found errors;
- filtering and searching list endpoints;
- returning paginated list responses;
- returning a club-focused dashboard summary;
- logging HTTP requests and important endpoint actions;
- running automated integration tests in GitHub Actions.

## Current Capabilities

### CRUD API

The application exposes CRUD endpoints for the main football entities:

- players;
- clubs;
- matches;
- competition standings.

### API Quality

The API includes:

- request validation with DataAnnotations;
- query parameter validation;
- reusable Minimal API validation filter;
- consistent `ValidationProblem` responses;
- structured `404` responses with ProblemDetails;
- Swagger/OpenAPI summaries and response metadata;
- filtering, search, and stable sorting for list endpoints;
- pagination for list endpoints with page metadata;
- read-only EF Core queries with `AsNoTracking()`;
- `CancellationToken` support for GET endpoints.

### Pagination

List endpoints return paginated responses.

```http
GET /api/players?page=1&pageSize=20
GET /api/clubs?page=1&pageSize=20
GET /api/matches?page=1&pageSize=20
GET /api/competition-standings?page=1&pageSize=20
```

Response shape:

```json
{
  "items": [],
  "page": 1,
  "pageSize": 20,
  "totalCount": 0,
  "totalPages": 0
}
```

Pagination defaults to `page = 1` and `pageSize = 20`. The maximum `pageSize` is `100`.

### Dashboard

The dashboard endpoint returns a compact overview for a club.

```http
GET /api/dashboard
GET /api/dashboard?clubId=1
```

If `clubId` is not provided, the API uses Tottenham as the default club with `clubId = 1`.

The dashboard response includes:

- club id and club name;
- player count;
- injured player count;
- upcoming matches;
- last matches;
- top scorers;
- top assists;
- players with most appearances.

### Logging

The application includes a request logging middleware and endpoint-specific logs.

Request logs include:

- HTTP method;
- path and query string;
- response status code;
- request duration.

Endpoint-specific logs cover important actions such as:

- creating, updating, and deleting resources;
- missing resources that result in `404`;
- dashboard summary requests and returned counts.

Log levels are intentionally simple:

- `Information` for successful requests and completed actions;
- `Warning` for expected client-side problems such as `404`;
- `Error` for `5xx` responses and unhandled exceptions.

### Tests And CI

The project includes integration tests for API behavior.

The tests cover:

- CRUD endpoints;
- dashboard responses;
- validation and not-found cases;
- pagination behavior;
- filtering behavior.

GitHub Actions runs the test suite on pushes and pull requests to `dev` and `main`. The CI workflow starts a temporary PostgreSQL service container, builds the solution, and runs `dotnet test`.

## Tech Stack

- C#;
- ASP.NET Core 8;
- Minimal APIs;
- Entity Framework Core;
- PostgreSQL;
- Npgsql;
- Swagger/OpenAPI;
- xUnit;
- ASP.NET Core integration testing;
- GitHub Actions.

## API Areas

| Area | Route |
| --- | --- |
| Players | `/api/players` |
| Clubs | `/api/clubs` |
| Matches | `/api/matches` |
| Competition standings | `/api/competition-standings` |
| Dashboard | `/api/dashboard` |

Examples:

```http
GET /api/players?clubId=1&search=son&isInjured=false&page=1&pageSize=20
GET /api/clubs?season=2025/26&page=1&pageSize=20
GET /api/matches?clubId=1&competition=Premier League&isHome=true&page=1&pageSize=20
GET /api/competition-standings?clubId=1&competition=Premier League&page=1&pageSize=20
GET /api/dashboard?clubId=1
```

## Project Structure

```text
.
├── .github/workflows/    GitHub Actions CI
├── src/TottenhamStatsAPI/
│   ├── Data/             EF Core DbContext
│   ├── DTOs/             request, response, query, and common DTOs
│   ├── Endpoints/        Minimal API endpoint groups
│   ├── Filters/          endpoint filters, including validation
│   ├── Helpers/          shared API response helpers
│   ├── Middleware/       request logging middleware
│   ├── Migrations/       EF Core migrations
│   ├── Models/           domain/database entities
│   └── Program.cs        application setup and endpoint registration
└── tests/
    └── TottenhamStatsAPI.Tests/
```

The project intentionally keeps the architecture simple. There is no service or repository layer yet because the current behavior is still mostly CRUD, read-only querying, pagination, and dashboard aggregation. Shared pagination response creation is centralized in `PagedResponse<T>`, while service/domain layers are postponed until real business rules make them useful.

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

## Running Tests

The integration tests use a separate PostgreSQL connection string named `TestConnection`.

Set it with user secrets for the test project:

```bash
dotnet user-secrets set "ConnectionStrings:TestConnection" "Host=localhost;Port=5432;Database=tottenham_stats_test;Username=postgres;Password=your_password" --project tests/TottenhamStatsAPI.Tests
```

Run tests:

```bash
dotnet test
```

The test factory applies EF Core migrations automatically before tests run.

## Development Status

Completed releases:

- `v0.1.0` - database schema and EF Core setup;
- `v0.2.0` - CRUD API;
- `v0.3.0` - API quality, validation, errors, OpenAPI metadata, filtering/search;
- `v0.4.0` - dashboard summary and basic logging;
- `v0.5.0` - integration tests, GitHub Actions CI, pagination, filtering tests, and pagination cleanup.

Current focus:

- `v0.6.0` - security and configuration.

See [ROADMAP.md](ROADMAP.md) for the planned version-by-version development path and [CHANGELOG.md](CHANGELOG.md) for released changes.

## Why This Project Exists

This repository is a learning and portfolio project. The emphasis is not only on adding features, but also on being able to explain:

- why the API is structured this way;
- how ASP.NET Core Minimal APIs handle routing, binding, filters, middleware, and responses;
- how EF Core queries are built and executed;
- how validation and error responses are represented for API clients;
- how request logging and endpoint-specific logs help understand application behavior;
- how integration tests and CI protect behavior during refactoring;
- how pagination keeps list endpoints predictable as data grows;
- how the project can grow without adding unnecessary architecture too early.
