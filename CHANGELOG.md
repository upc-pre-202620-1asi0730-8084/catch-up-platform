# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2026-10-27

### Added

- **News Bounded Context**:
  - `FavoriteSource` aggregate root with `NewsApiKey` and `SourceId` value objects with domain validation.
  - RESTful API endpoints under `/api/v1/favorite-sources` using lowercase kebab-case routing:
    - `POST /api/v1/favorite-sources` to register a new favorite news source.
    - `GET /api/v1/favorite-sources/{id}` to retrieve a favorite source by its unique identifier.
    - `GET /api/v1/favorite-sources?newsApiKey={key}` to list all favorite sources associated with an API key.
    - `GET /api/v1/favorite-sources?newsApiKey={key}&sourceId={sourceId}` to retrieve a specific favorite source.
  - Command and Query Separation (CQS) architecture with `IFavoriteSourceCommandService` and `IFavoriteSourceQueryService`.
  - Inbound and outbound resource DTOs (`CreateFavoriteSourceResource`, `FavoriteSourceResource`).
  - Static assembler classes (`CreateFavoriteSourceCommandFromResourceAssembler`, `FavoriteSourceResourceFromEntityAssembler`, `ActionResultFromCreateFavoriteSourceResultAssembler`) for mapping between HTTP requests, commands, and responses.
  - Domain repository interface `IFavoriteSourceRepository` and Entity Framework Core persistence implementation `FavoriteSourceRepository`.
  - Uniqueness constraint check and `409 Conflict` response for duplicate `(newsApiKey, sourceId)` pairs.
- **Shared Bounded Context**:
  - Generic repository pattern (`IBaseRepository<TEntity>`, `BaseRepository<TEntity>`) and Unit of Work (`IUnitOfWork`, `UnitOfWork`) for transaction management.
  - Functional `Result<TValue, TError>` pattern to model explicit success and failure outcomes without relying on exceptions for control flow.
  - Entity audit trail support with `IAuditableEntity` and `AuditableEntityInterceptor` for automatic `CreatedAt` and `UpdatedAt` timestamp management on save changes.
  - Custom kebab-case routing convention (`KebabCaseRouteNamingConvention`) and snake_case database naming convention (`ModelBuilderExtensions.UseSnakeCaseNamingConvention`).
- **API Documentation & Localization**:
  - Interactive API documentation using Scalar at `/scalar` and built-in ASP.NET Core OpenAPI document generation at `/openapi/v1.json`.
  - Multi-language localization support for error messages, data annotations, and OpenAPI metadata across `en` (default), `en-US`, `es`, and `es-PE` cultures via `.resx` resource files (`SharedResource`).
  - Standardized RFC 7807 `ProblemDetails` error responses for HTTP 400 validation errors and HTTP 500 unexpected server exceptions.
- **Infrastructure & Deployment**:
  - Entity Framework Core 10 integration with MySQL provider (`MySql.EntityFrameworkCore`).
  - Environment variable resolution in connection strings (`appsettings.Production.json`) for production configuration.
  - Automatic database migration execution on startup via `Database.Migrate()`.
  - Containerization support with multi-stage `Dockerfile`.
- **Project Documentation**:
  - Architecture Decision Records (`docs/adrs.md`) documenting 13 architectural decisions (ADR-001 to ADR-013).
  - PlantUML class diagram (`docs/class-diagram.puml`) illustrating domain models, bounded contexts, and service interactions.
  - Technical user stories specification (`docs/user-stories.md`).
  - Comprehensive `README.md` and MIT License (`LICENSE.md`).

### Security

- Implemented `NewsApiKey.MaskedForLogging` (instance and static methods) to ensure third-party API credentials are sanitized in application logs.
