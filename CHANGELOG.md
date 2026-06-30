# Changelog

## v0.5.0 - Tests, CI & Pagination

### Added

- Integration test project with xUnit and ASP.NET Core `WebApplicationFactory`
- Test database setup through `ConnectionStrings:TestConnection`
- Automatic EF Core migrations during integration test startup
- Integration tests for CRUD endpoints, dashboard, validation and not-found cases
- Pagination support for list endpoints with `page`, `pageSize`, `totalCount` and `totalPages`
- Pagination tests for players, clubs, matches and competition standings
- Filtering tests for players, clubs, matches and competition standings
- GitHub Actions CI workflow for pushes and pull requests to `dev` and `main`
- PostgreSQL service container for CI test runs
- Reusable `PaginationParameters`
- Reusable `PagedResponse<T>`

### Changed

- List endpoints now return paged response objects instead of raw arrays
- Centralized paged response creation in `PagedResponse<T>.Create(...)`
- Kept service/domain layers out of the release until real business logic appears
- Postponed deeper unit testing until domain rules are introduced
- Updated `coverlet.collector` after a security warning

## v0.4.0 - Dashboard & Basic Logging

### Added

- Dashboard endpoint for club-level summary data
- Dashboard query parameter with default `clubId = 1`
- Dashboard response DTOs for matches and player statistics
- Upcoming matches, last matches, top scorers, top assists and most appearances in the dashboard response
- Player count and injured player count in the dashboard response
- Request logging middleware for HTTP method, path, query string, status code and duration
- Endpoint-specific logs for create, update, delete and not-found cases
- Dashboard-specific logs for summary requests and returned counts

### Changed

- Moved the project focus from API quality to dashboard/statistics and basic application visibility
- Kept separate statistics endpoints out of the release to avoid duplicating dashboard behavior

## v0.3.0 - API Quality

### Added

- Request validation with DataAnnotations
- Reusable Minimal API validation filter
- Query parameter DTOs for list endpoints
- Query validation for filtering/search parameters
- Filtering and search for players, clubs, matches and competition standings
- Stable sorting for list endpoints
- OpenAPI summaries and response metadata
- `CancellationToken` support for GET endpoints

### Changed

- Improved `404` responses with ProblemDetails
- Used `AsNoTracking()` for read-only EF Core queries
- Moved API behavior closer to a predictable client-facing HTTP API

## v0.2.0 - CRUD API

### Added

- Player CRUD endpoints
- Club CRUD endpoints
- Match CRUD endpoints
- Competition standing CRUD endpoints
- DTOs for API requests and responses
- Swagger UI setup

### Fixed

- Player update route handling
- NotFound responses for missing resources
- Removed local database password from appsettings

## v0.1.0 - Database Schema

### Added

- ASP.NET Core project structure
- PostgreSQL connection
- EF Core DbContext
- Club, Player, Match and CompetitionStanding entities
- Initial migrations
