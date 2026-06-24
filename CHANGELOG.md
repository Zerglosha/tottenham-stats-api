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
