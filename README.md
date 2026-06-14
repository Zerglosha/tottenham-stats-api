# Tottenham Stats API

ASP.NET Core backend API for manually tracking Tottenham Hotspur statistics.

## Tech stack

- ASP.NET Core
- PostgreSQL
- EF Core
- Swagger/OpenAPI

## MVP

- Players statistics
- Matches and results
- Club information
- Competition standings
- Dashboard summary

# Roadmap:

## v0.1.0 — Database Schema ✅

### Added

* ASP.NET Core project structure
* PostgreSQL integration
* Entity Framework Core setup
* AppDbContext configuration
* Initial database migrations
* Club entity
* Player entity
* Match entity
* CompetitionStanding entity
* Relationships and foreign keys

### Goal

Create and persist the domain model in PostgreSQL.

---

## v0.2.0 — CRUD API ✅

### Added

* Player CRUD endpoints
* Club CRUD endpoints
* Match CRUD endpoints
* CompetitionStanding CRUD endpoints
* DTOs for requests and responses
* Swagger/OpenAPI UI
* Route groups
* NotFound handling
* Proper HTTP status codes

### Goal

Allow full management of application data through HTTP API.

---

## v0.3.0 — API Quality 🚧

### Planned

* Validation attributes
* Request validation
* OpenAPI descriptions
* Endpoint summaries
* Better error responses
* Filtering
* Sorting
* Search endpoints

### Goal

Transform CRUD endpoints into a production-style API.

---

## v0.4.0 — Dashboard & Statistics

### Planned

* Dashboard endpoint
* Top scorers
* Top assists
* Most appearances
* Injured players list
* Last 5 matches
* Upcoming matches
* Club overview endpoint

### Goal

Provide aggregated football statistics instead of only CRUD operations.

---

## v0.5.0 — Architecture Improvements

### Planned

* Service layer
* Mapping helpers
* Shared response models
* Cleaner endpoint organization
* Reduce duplicated code

### Goal

Improve maintainability and scalability of the codebase.

---

## v0.6.0 — Security Fundamentals

### Planned

* Global exception handler
* Rate limiting
* CORS configuration
* User secrets
* Environment-based configuration
* Secure error handling

### Goal

Introduce backend security best practices.

---

## v0.7.0 — Authentication & Authorization

### Planned

* JWT authentication
* Admin role
* Protected CRUD operations
* Public read endpoints
* Authorization policies

### Goal

Separate public and administrative functionality.

---

## v0.8.0 — Data Integrity

### Planned

* Domain validation rules
* Match status validation
* Standing consistency checks
* Enum-based statuses and competitions
* Seed data

### Goal

Prevent invalid football data from entering the system.

---

## v0.9.0 — Deployment

### Planned

* Docker support
* Docker Compose
* PostgreSQL container
* Environment configuration
* Production deployment

### Goal

Run the application outside of the development machine.

---

## v1.0.0 — First Complete MVP

### Planned

* Stable API
* Dashboard
* Authentication
* Validation
* Security
* Deployment
* Documentation

### Goal

Complete backend MVP ready for portfolio presentation.
