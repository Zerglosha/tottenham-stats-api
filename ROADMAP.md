# Roadmap

This roadmap keeps the project focused on useful backend milestones instead of adding technology for its own sake.

## v0.1.0 - Database Schema

Status: released.

Added:

- ASP.NET Core project structure;
- PostgreSQL integration;
- Entity Framework Core setup;
- `AppDbContext` configuration;
- initial database migrations;
- `Club` entity;
- `Player` entity;
- `Match` entity;
- `CompetitionStanding` entity;
- relationships and foreign keys.

Goal: create and persist the initial football domain model in PostgreSQL.

## v0.2.0 - CRUD API

Status: released.

Added:

- Player CRUD endpoints;
- Club CRUD endpoints;
- Match CRUD endpoints;
- CompetitionStanding CRUD endpoints;
- request and response DTOs;
- Swagger/OpenAPI UI;
- route groups;
- basic `NotFound` handling;
- proper HTTP status codes for CRUD operations.

Goal: allow full management of application data through HTTP API.

## v0.3.0 - API Quality

Status: released.

Added:

- request validation with DataAnnotations;
- reusable `ValidationFilter<T>` for request and query validation;
- consistent validation errors with `ValidationProblem`;
- improved `404` responses with ProblemDetails;
- OpenAPI summaries and response metadata;
- filtering and search for list endpoints;
- query validation;
- stable sorting for list endpoints;
- `AsNoTracking()` for read-only EF Core queries;
- `CancellationToken` support for GET endpoints.

Goal: transform the basic CRUD API into a more predictable, documented, and client-friendly HTTP API.

## v0.4.0 - Dashboard & Basic Logging

Status: released.

Added:

- club-focused dashboard endpoint;
- dashboard query parameter with default `clubId = 1`;
- dashboard response DTOs;
- upcoming matches for a club;
- last matches for a club;
- top scorers for a club;
- top assists for a club;
- players with most appearances for a club;
- player and injured player counts;
- request logging middleware;
- endpoint-specific logs for create, update, delete, not-found cases, and dashboard summary.

Endpoint:

```http
GET /api/dashboard
GET /api/dashboard?clubId=1
```

The dashboard is intentionally a compact aggregate endpoint. Separate statistics endpoints and club overview endpoints are postponed because they would currently duplicate the dashboard response or existing filtered CRUD endpoints.

Goal: start returning useful club-level football statistics and add enough logging to understand what the application is doing during local development.

## v0.5.0 - Tests & Architecture

Status: released.

Added:

- integration test project with xUnit and `WebApplicationFactory`;
- test database setup through `ConnectionStrings:TestConnection`;
- automatic EF Core migrations for integration tests;
- integration tests for CRUD endpoints, dashboard, validation, not-found cases, pagination, and filtering;
- GitHub Actions CI for pull requests and pushes to `dev` and `main`;
- PostgreSQL service container in CI;
- pagination for list endpoints with `page`, `pageSize`, `totalCount`, and `totalPages`;
- reusable `PaginationParameters`;
- reusable `PagedResponse<T>`;
- centralized paged response creation with `PagedResponse<T>.Create(...)`;
- dependency update for test coverage tooling after a security warning;
- decision to keep service/domain layers out until real business logic appears;
- decision to postpone deeper unit testing until domain rules exist.

Goal: improve maintainability and introduce reliable automated verification.

## v0.6.0 - Security & Configuration

Status: current focus.

Planned:

- global exception handler;
- CORS configuration;
- rate limiting;
- user secrets;
- environment-based configuration;
- secure error handling.

Goal: add basic backend security practices before authentication.

## v0.7.0 - Authentication & Authorization

Status: planned.

Planned:

- JWT authentication;
- admin role;
- protected write endpoints;
- public read endpoints;
- authorization policies.

Goal: separate public read access from administrative data management.

## v0.7.5 - Data Integrity & Domain Rules

Status: planned.

Planned:

- football-specific data integrity rules;
- consistency checks for competition standings;
- `Played = Wins + Draws + Losses`;
- `GoalDifference = GoalsFor - GoalsAgainst`;
- `Points = Wins * 3 + Draws` for league tables where the three-points-for-a-win rule applies;
- match status rules, such as completed matches requiring scores and scheduled matches allowing empty scores;
- player statistics rules, such as player appearances not exceeding the club's available played matches for the relevant scope;
- extraction of domain validation logic into testable classes or services;
- unit tests for domain rules;
- integration tests for API behavior when domain rules fail.

Goal: add meaningful football domain logic after the API has tests, security, and authorization in place, giving the project stronger correctness guarantees and a natural place for unit tests.

## v0.8.0 - Deployment

Status: planned.

Planned:

- Docker support;
- Docker Compose;
- PostgreSQL container;
- environment configuration;
- production deployment.

Goal: run the application outside the local development machine.

## v0.9.0 - Observability & Profiling

Status: planned.

Planned:

- structured application logging;
- health checks;
- basic metrics for API requests and database access;
- slow query investigation;
- EF Core query logging/profiling in development;
- simple performance checks for list and dashboard endpoints;
- documentation for how to inspect application behavior locally.

Goal: understand how the application behaves while it is running, detect problems earlier, and learn how to investigate performance bottlenecks instead of guessing.

## v1.0.0 - Portfolio MVP

Status: planned.

Planned:

- stable API;
- dashboard summary;
- validation;
- tests;
- security basics;
- authentication;
- data integrity and domain rules;
- deployment;
- observability and profiling basics;
- documentation.

Goal: complete a backend MVP that can be confidently presented as a portfolio project.

## After v1.0.0

Possible future work:

- separate statistics endpoints such as top scorers, top assists, and most appearances;
- richer dashboard queries;
- frontend dashboard;
- product-oriented football analytics.

These are intentionally postponed until there is a real consumer for those endpoints. At the current stage, the dashboard already exposes this data and separate statistics routes would duplicate behavior.

## Data Integrity Notes

Data integrity should not be delayed until the end of the roadmap. Simple rules are already part of validation, and deeper football-specific rules are planned for `v0.7.5`, after tests, security, and authorization are in place.

Examples:

- `Played = Wins + Draws + Losses`;
- `GoalDifference = GoalsFor - GoalsAgainst`;
- `Points = Wins * 3 + Draws` where the competition uses that points system;
- player appearances should not exceed the club's available played matches for the relevant scope;
- match status must be one of the allowed values;
- completed matches should have scores;
- scheduled matches may have no scores;
- enum-based statuses and competitions can be introduced when raw strings start hurting maintainability.
