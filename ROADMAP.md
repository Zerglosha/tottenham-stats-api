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

## v0.4.0 - Dashboard & Statistics

Status: current focus.

Planned:

- dashboard summary endpoint;
- top scorers endpoint;
- top assists endpoint;
- most appearances endpoint;
- injured players endpoint;
- last matches endpoint;
- upcoming matches endpoint;
- club overview endpoint;
- dedicated response DTOs for statistics scenarios;
- read-only queries with `AsNoTracking()` and `CancellationToken`;
- basic application logging;
- request logging;
- logging for important dashboard/statistics queries and unexpected errors.

Possible route ideas:

- `GET /api/dashboard/summary`;
- `GET /api/statistics/top-scorers`;
- `GET /api/statistics/top-assists`;
- `GET /api/statistics/most-appearances`;
- `GET /api/statistics/injured-players`;
- `GET /api/statistics/last-matches`;
- `GET /api/statistics/upcoming-matches`;
- `GET /api/clubs/{clubId}/overview`.

Goal: start returning useful football statistics instead of only CRUD-shaped data, while adding enough logging to understand what the application is doing during local development.

## v0.5.0 - Tests & Architecture

Planned:

- mapping helpers;
- cleaner endpoint organization;
- service layer only where real business logic appears;
- pagination for list endpoints;
- unit tests for domain logic;
- integration tests for key endpoints;
- test database setup;
- automated API checks;
- optional Postman/Newman or `.http` files for repeatable API scenarios.

Goal: improve maintainability and introduce reliable automated verification.

## v0.6.0 - Security & Configuration

Planned:

- global exception handler;
- CORS configuration;
- rate limiting;
- user secrets;
- environment-based configuration;
- secure error handling.

Goal: add basic backend security practices before authentication.

## v0.7.0 - Authentication & Authorization

Planned:

- JWT authentication;
- admin role;
- protected write endpoints;
- public read endpoints;
- authorization policies.

Goal: separate public read access from administrative data management.

## v0.8.0 - Deployment

Planned:

- Docker support;
- Docker Compose;
- PostgreSQL container;
- environment configuration;
- production deployment.

Goal: run the application outside the local development machine.

## v0.9.0 - Observability & Profiling

Planned:

- structured application logging;
- request logging;
- health checks;
- basic metrics for API requests and database access;
- slow query investigation;
- EF Core query logging/profiling in development;
- simple performance checks for list and statistics endpoints;
- documentation for how to inspect application behavior locally.

Goal: understand how the application behaves while it is running, detect problems earlier, and learn how to investigate performance bottlenecks instead of guessing.

## v1.0.0 - Portfolio MVP

Planned:

- stable API;
- dashboard/statistics;
- validation;
- tests;
- security basics;
- authentication;
- deployment;
- observability and profiling basics;
- documentation.

Goal: complete a backend MVP that can be confidently presented as a portfolio project.

## Data Integrity Notes

Data integrity should not be delayed until the end of the roadmap. Simple rules are already part of validation, and deeper football-specific rules can be added when statistics and tests make them easier to protect.

Examples for future versions:

- `Played = Wins + Draws + Losses`;
- `GoalDifference = GoalsFor - GoalsAgainst`;
- match status must be one of the allowed values;
- completed matches should have scores;
- scheduled matches may have no scores;
- enum-based statuses and competitions can be introduced when raw strings start hurting maintainability.
