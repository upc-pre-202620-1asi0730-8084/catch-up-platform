# Catch-Up Platform — Study Guide

> **Purpose:** This guide is designed for Web Applications students who want to understand the approaches, patterns, frameworks, and language features applied in this RESTful API example. Each topic includes a detailed explanation, a reference to the code file(s) where it is used, and external learning resources.

---

## Table of Contents

1. [Architecture & Design](#1-architecture--design)
   - 1.1. Clean Architecture / Layered Architecture
   - 1.2. Domain-Driven Design (DDD) — Bounded Contexts
   - 1.3. CQS (Command Query Separation)
2. [Design Patterns](#2-design-patterns)
   - 2.1. Repository Pattern
   - 2.2. Unit of Work Pattern
   - 2.3. Result Pattern (Either Type)
   - 2.4. Value Object Pattern
   - 2.5. Aggregate Root Pattern
   - 2.6. Assembler / Transformer Pattern
   - 2.7. Interceptor Pattern
   - 2.8. Convention Pattern
   - 2.9. Partial Class Pattern
3. [ASP.NET Core Framework Features](#3-aspnet-core-framework-features)
   - 3.1. Minimal Hosting Model
   - 3.2. Dependency Injection
   - 3.3. Model Binding & Validation
   - 3.4. ProblemDetails (RFC 7807)
   - 3.5. Localization with `.resx` Resources
   - 3.6. OpenAPI Generation & Scalar
   - 3.7. Routing Conventions
   - 3.8. Structured Logging
4. [Entity Framework Core](#4-entity-framework-core)
   - 4.1. Code-First Migrations
   - 4.2. Value Converter Mapping
   - 4.3. Save Changes Interceptors
   - 4.4. Snake Case Naming Convention
   - 4.5. Automatic Migration on Startup
5. [C# Language Features](#5-c-language-features)
   - 5.1. Records
   - 5.2. Primary Constructors
   - 5.3. Pattern Matching
   - 5.4. Nullable Reference Types
   - 5.5. `using` Directives (Implicit Usings)
   - 5.6. Source Generators (`[GeneratedRegex]`)
   - 5.7. `when` Exception Filters
   - 5.8. String Slicing (`^4..`)
   - 5.9. `sealed` Classes and Records
   - 5.10. `private set` Properties
   - 5.11. `static abstract` Members via Records
   - 5.12. `IEnumerable<char>` with Local Functions
6. [DevOps & Containerization](#6-devops--containerization)
   - 6.1. Multi-Stage Dockerfile
   - 6.2. Environment Variable Expansion
7. [Recommended Learning Path](#7-recommended-learning-path)
8. [External References](#8-external-references)

---

## 1. Architecture & Design

### 1.1. Clean Architecture / Layered Architecture

**What it is:** Clean Architecture organizes code into concentric layers where the innermost layer contains the business rules (Domain), and outer layers depend on inner layers — never the reverse. This project uses a practical variant: each bounded context is split into `Domain`, `Application`, `Infrastructure`, and `Interfaces` folders.

**Why it matters:** It keeps business logic independent of frameworks (ASP.NET Core, EF Core, MySQL). You can swap the database or the web framework without touching the domain model.

**Where in the codebase:**

| Layer | Folder | Example files |
|-------|--------|---------------|
| **Domain** | `News/Domain/` | `FavoriteSource.cs`, `NewsApiKey.cs`, `IFavoriteSourceRepository.cs` |
| **Application** | `News/Application/` | `IFavoriteSourceCommandService.cs`, `FavoriteSourceCommandService.cs` |
| **Infrastructure** | `News/Infrastructure/` | `FavoriteSourceRepository.cs` |
| **Interfaces** | `News/Interfaces/` | `FavoriteSourcesController.cs`, `CreateFavoriteSourceResource.cs` |
| **Shared** | `Shared/` | `Result.cs`, `IBaseRepository.cs`, `IUnitOfWork.cs` |

**Key rule:** Dependencies point inward. `Interfaces` → `Application` → `Domain`. `Infrastructure` implements Domain contracts.

**Learn more:**
- [Clean Architecture by Robert C. Martin (Uncle Bob)](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Microsoft: Application Architecture — Layered Architecture](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/choose-between-microservices-monolith)

---

### 1.2. Domain-Driven Design (DDD) — Bounded Contexts

**What it is:** DDD is an approach to software design that focuses on modeling the software after the business domain. A *Bounded Context* is a explicit boundary within which a particular domain model and its associated language (Ubiquitous Language) apply.

**Why it matters:** As the application grows, different features may use the same word with different meanings. Bounded contexts prevent confusion and keep each model coherent.

**Where in the codebase:**

The project defines two bounded contexts:

- **`News`** — owns the favorite-source use cases, aggregate, commands, queries, and API contracts.
- **`Shared`** — owns cross-cutting abstractions used by multiple contexts (repository contracts, unit of work, auditing, Result pattern).

**File structure:**
```
CatchUpPlatform.API/
├── News/                    ← News Bounded Context
│   ├── Domain/
│   ├── Application/
│   ├── Infrastructure/
│   └── Interfaces/
└── Shared/                  ← Shared Bounded Context
    ├── Application/
    ├── Domain/
    └── Infrastructure/
```

**Learn more:**
- [Domain-Driven Design: Tackling Complexity in the Heart of Software (Evans)](https://www.informit.com/store/domain-driven-design-tackling-complexity-in-the-heart-of/9780321125217)
- [DDD Bounded Contexts — Martin Fowler](https://martinfowler.com/bliki/BoundedContext.html)

---

### 1.3. CQS (Command Query Separation)

**What it is:** CQS is a principle stating that a method should either modify state (Command) or return data (Query), but not both. This project applies CQS at the service level by separating `IFavoriteSourceCommandService` from `IFavoriteSourceQueryService`.

**Why it matters:** Commands and queries have different concerns. Commands need validation, uniqueness checks, and transaction management. Queries are simple lookups. Separating them makes each path easier to reason about and evolve independently.

**Where in the codebase:**

- **Command Service:** `IFavoriteSourceCommandService.cs` → `FavoriteSourceCommandService.cs`
  - Handles `CreateFavoriteSourceCommand`
  - Returns `Result<FavoriteSource, ECreateFavoriteSourceError>`
- **Query Service:** `IFavoriteSourceQueryService.cs` → `FavoriteSourceQueryService.cs`
  - Handles `GetFavoriteSourceByIdQuery`, `GetAllFavoriteSourcesByNewsApiKeyQuery`, `GetFavoriteSourceByNewsApiKeyAndSourceIdQuery`
  - Returns entities or collections directly

**Important note:** This is CQS at the service level, not full CQRS. Both sides share the same database and model.

**Learn more:**
- [Command-Query Separation — Wikipedia](https://en.wikipedia.org/wiki/Command%E2%80%93query_separation)
- [CQRS — Microsoft Docs](https://learn.microsoft.com/en-us/azure/architecture/patterns/cqrs)

---

## 2. Design Patterns

### 2.1. Repository Pattern

**What it is:** The Repository pattern acts as an intermediary between the domain layer and the data mapping layer. It provides a collection-like interface for accessing domain objects, hiding the persistence details.

**Why it matters:** It decouples your business logic from the database technology. You can swap MySQL for PostgreSQL or add an in-memory repository for testing without changing the domain code.

**Where in the codebase:**

- **Domain contract:** `IFavoriteSourceRepository.cs` extends `IBaseRepository<FavoriteSource>`
- **Shared base contract:** `IBaseRepository.cs` defines `AddAsync`, `FindByIdAsync`, `Update`, `Remove`, `ListAsync`
- **Base implementation:** `BaseRepository.cs` provides generic EF Core CRUD operations
- **Concrete implementation:** `FavoriteSourceRepository.cs` extends `BaseRepository<FavoriteSource>` and adds domain-specific queries (`FindByNewsApiKeyAsync`, `FindByNewsApiKeyAndSourceIdAsync`)

**Key insight:** The domain layer depends on the interface (`IFavoriteSourceRepository`), not on EF Core. The infrastructure layer provides the EF Core implementation.

**Learn more:**
- [Repository Pattern — Microsoft Docs](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/requests-commands-commands-handlers)
- [Repository Pattern — Martin Fowler](https://martinfowler.com/eaaCatalog/repository.html)

---

### 2.2. Unit of Work Pattern

**What it is:** The Unit of Work pattern maintains a list of objects affected by a business transaction and coordinates the writing out of changes. It ensures that all changes are committed atomically — either all succeed or all fail.

**Why it matters:** Without Unit of Work, each repository call might commit independently, leading to partial updates and data inconsistency.

**Where in the codebase:**

- **Domain contract:** `IUnitOfWork.cs` — defines `CompleteAsync()`
- **Implementation:** `UnitOfWork.cs` — wraps `context.SaveChangesAsync()`
- **Usage:** In `FavoriteSourceCommandService.cs`:
  ```csharp
  await favoriteSourceRepository.AddAsync(favoriteSource, cancellationToken);
  await unitOfWork.CompleteAsync(cancellationToken);
  ```

**Learn more:**
- [Unit of Work Pattern — Martin Fowler](https://martinfowler.com/eaaCatalog/unitOfWork.html)

---

### 2.3. Result Pattern (Either Type)

**What it is:** The Result pattern (also called the Either type) is a way to represent the outcome of an operation that can either succeed with a value or fail with an error — without using exceptions for expected failures.

**Why it matters:** Exceptions are expensive and obscure control flow. When a failure is an expected business outcome (e.g., "duplicate favorite source"), returning a `Result` makes the failure path explicit in the method signature and forces the caller to handle it.

**Where in the codebase:**

- **Definition:** `Result.cs` — an abstract record with two sealed subtypes: `Success` and `Failure`
- **Error enum:** `ECreateFavoriteSourceError.cs` — enumerates the possible failure reasons
- **Usage in command handler:** `FavoriteSourceCommandService.cs` returns `Result<FavoriteSource, ECreateFavoriteSourceError>`
- **Usage in assembler:** `ActionResultFromCreateFavoriteSourceResultAssembler.cs` maps the Result to HTTP status codes

**Key methods on `Result<TValue, TError>`:**

| Method | Purpose |
|--------|---------|
| `IsSuccess` / `IsFailure` | Check the outcome |
| `Map<TNext>(Func<TValue, TNext>)` | Transform the success value (functor) |
| `Fold<TResult>(onSuccess, onFailure)` | Exhaustively match both branches |
| `Match(onSuccess, onFailure)` | Execute an action on the matching branch |

**Learn more:**
- [Functional Programming: The Result Type](https://fsharpforfunandprofit.com/rop/)
- [Railway-Oriented Programming — Scott Wlaschin](https://fsharpforfunandprofit.com/posts/recipe-part2/)

---

### 2.4. Value Object Pattern

**What it is:** A Value Object is an immutable object that is defined solely by the values of its properties, not by its identity. Two value objects with the same values are considered equal.

**Why it matters:** Value objects encapsulate validation rules and behavior with the data they represent. Instead of passing raw strings (which can be null, empty, or mixed up), you pass typed objects that guarantee validity.

**Where in the codebase:**

- **`NewsApiKey`** — validates non-empty, max 255 chars, provides `MaskedForLogging()`
- **`SourceId`** — validates non-empty, max 255 chars

Both are implemented as `sealed record` types with constructor validation.

**Key benefits demonstrated:**
1. **Type safety:** The compiler prevents passing `sourceId` where `newsApiKey` is expected.
2. **Validation at construction:** Invalid values cannot enter the domain.
3. **Behavior with data:** `MaskedForLogging()` lives on the value object, not scattered across log calls.

**Learn more:**
- [Value Object — Martin Fowler](https://martinfowler.com/bliki/ValueObject.html)
- [Value Objects in DDD](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/domain-events-design-implementation)

---

### 2.5. Aggregate Root Pattern

**What it is:** An Aggregate Root is a domain entity that acts as the gateway to a cluster of related entities (the aggregate). All access to the aggregate's internal objects goes through the root, ensuring consistency.

**Why it matters:** It defines a clear boundary for transactions and invariants. You load, modify, and save the entire aggregate through the root.

**Where in the codebase:**

- **`FavoriteSource`** is the aggregate root in the `News` bounded context.
- It has a parameterless constructor (required by EF Core) and a constructor that accepts a `CreateFavoriteSourceCommand`.
- Properties have `private set` to enforce encapsulation — only the aggregate itself can modify its state.

**Learn more:**
- [Aggregates in DDD — Vladimir Khorikov](https://enterprisecraftsmanship.com/posts/ddd-aggregates-ensure-consistency-available-updates/)

---

### 2.6. Assembler / Transformer Pattern

**What it is:** Assemblers (also called Mappers or Transformers) convert objects from one representation to another — for example, from a REST resource (DTO) to a domain command, or from a domain entity to an HTTP response resource.

**Why it matters:** It keeps the boundary between layers clean. Domain entities never leak into the REST layer, and REST DTOs never enter the domain.

**Where in the codebase:**

| Assembler | Converts | File |
|-----------|----------|------|
| `CreateFavoriteSourceCommandFromResourceAssembler` | `CreateFavoriteSourceResource` → `CreateFavoriteSourceCommand` | `CreateFavoriteSourceCommandFromResourceAssembler.cs` |
| `FavoriteSourceResourceFromEntityAssembler` | `FavoriteSource` → `FavoriteSourceResource` | `FavoriteSourceResourceFromEntityAssembler.cs` |
| `ActionResultFromCreateFavoriteSourceResultAssembler` | `Result<FavoriteSource, ...>` → `ActionResult` | `ActionResultFromCreateFavoriteSourceResultAssembler.cs` |
| `LocalizedOpenApiAssembler` | OpenAPI operation/schema → localized text | `LocalizedOpenApiAssembler.cs` |

All assemblers are `static class` types — they have no state and are pure transformation functions.

**Learn more:**
- [AutoMapper vs Manual Mapping](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/requests-commands-commands-handlers)

---

### 2.7. Interceptor Pattern

**What it is:** An Interceptor hooks into a framework's lifecycle to execute custom logic before or after a specific operation, without modifying the operation's code.

**Why it matters:** It lets you add cross-cutting concerns (like auditing) in one place, rather than repeating the same logic in every repository or service.

**Where in the codebase:**

- **`AuditableEntityInterceptor.cs`** — implements `SaveChangesInterceptor` from EF Core.
- Before every `SaveChanges`, it iterates over tracked entities that implement `IAuditableEntity` and sets `CreatedAt` (on `Added`) and `UpdatedAt` (on `Added` or `Modified`).
- Registered in `AppDbContext.OnConfiguring()`.

**Learn more:**
- [EF Core Interceptors](https://learn.microsoft.com/en-us/ef/core/logging-diagnostics/interceptors)

---

### 2.8. Convention Pattern

**What it is:** A Convention is a rule applied automatically to a set of elements based on their structure, rather than explicit configuration on each one.

**Why it matters:** It reduces boilerplate. Instead of decorating every route or column with an attribute, a convention applies the rule globally.

**Where in the codebase:**

- **`KebabCaseRouteNamingConvention.cs`** — implements `IControllerModelConvention` to transform `[controller]` route tokens from PascalCase to kebab-case (e.g., `FavoriteSourcesController` → `/api/v1/favorite-sources`).
- **`ModelBuilderExtensions.cs`** — applies `snake_case` naming to all tables, columns, keys, and indexes.

**Learn more:**
- [ASP.NET Core MVC Conventions](https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/conventions)

---

### 2.9. Partial Class Pattern

**What it is:** A `partial` class splits the definition of a class across multiple files. The compiler merges them into a single class at compile time.

**Why it matters:** It lets you separate concerns within a single type. In this project, the aggregate root's core properties live in one file, while the audit fields (`CreatedAt`, `UpdatedAt`) live in another.

**Where in the codebase:**

- `FavoriteSource.cs` — core properties (`Id`, `NewsApiKey`, `SourceId`)
- `FavoriteSourceAudit.cs` — audit fields (`CreatedAt`, `UpdatedAt`) implementing `IAuditableEntity`

**Learn more:**
- [Partial Classes and Methods (C#)](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/partial-class)

---

## 3. ASP.NET Core Framework Features

### 3.1. Minimal Hosting Model

**What it is:** Starting with .NET 6, ASP.NET Core introduced a minimal hosting model that replaces the traditional `Startup.cs` + `Program.cs` pattern with a single `Program.cs` file using top-level statements.

**Why it matters:** It reduces boilerplate and makes the application entry point easier to read. All configuration happens in one place.

**Where in the codebase:**

- `Program.cs` — the entire application is configured and started in a single file:
  - `WebApplication.CreateBuilder(args)` creates the builder
  - `builder.Services.Add*()` registers services
  - `var app = builder.Build()` builds the application
  - `app.Use*()` and `app.Map*()` configure the middleware pipeline
  - `app.Run()` starts the server

**Learn more:**
- [ASP.NET Core Generic Host](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host)

---

### 3.2. Dependency Injection

**What it is:** Dependency Injection (DI) is a pattern where objects receive their dependencies from an external source rather than creating them internally. ASP.NET Core has a built-in DI container.

**Why it matters:** It promotes loose coupling, testability, and easier configuration. You can swap implementations without changing the consuming code.

**Where in the codebase:**

In `Program.cs`:
```csharp
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IFavoriteSourceRepository, FavoriteSourceRepository>();
builder.Services.AddScoped<IFavoriteSourceCommandService, FavoriteSourceCommandService>();
builder.Services.AddScoped<IFavoriteSourceQueryService, FavoriteSourceQueryService>();
```

**Service lifetimes used:**
- `AddScoped` — one instance per HTTP request (used for all services and repositories)

**Primary Constructor Injection:** Controllers and services use C# primary constructors to declare dependencies:
```csharp
public class FavoriteSourceCommandService(
    IFavoriteSourceRepository favoriteSourceRepository,
    IUnitOfWork unitOfWork,
    ILogger<FavoriteSourceCommandService> logger) : IFavoriteSourceCommandService
```

**Learn more:**
- [Dependency Injection in .NET](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection)

---

### 3.3. Model Binding & Validation

**What it is:** Model Binding automatically maps incoming HTTP request data (query strings, body, route parameters) to C# objects. Validation attributes (Data Annotations) enforce rules on those objects.

**Why it matters:** It eliminates manual parsing and validation code in controllers.

**Where in the codebase:**

- **`CreateFavoriteSourceResource.cs`** — uses `[Required]` with localized error messages:
  ```csharp
  public record CreateFavoriteSourceResource(
      [Required(ErrorMessage = "RequiredNewsApiKey")] string NewsApiKey,
      [Required(ErrorMessage = "RequiredSourceId")] string SourceId);
  ```
- **`FavoriteSourcesController.cs`** — uses `[FromBody]` and `[FromQuery]` attributes for model binding.
- **Custom validation error handler** in `Program.cs` — overrides `InvalidModelStateResponseFactory` to return localized `ProblemDetails`.

**Learn more:**
- [Model Binding in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/mvc/models/model-binding)
- [Data Annotations Validation](https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation)

---

### 3.4. ProblemDetails (RFC 7807)

**What it is:** RFC 7807 defines a standard JSON format for error responses in HTTP APIs. ASP.NET Core implements this via `ProblemDetails`.

**Why it matters:** It provides a consistent, machine-readable error format that clients can parse reliably.

**Where in the codebase:**

- **`Program.cs`** — configures `AddProblemDetails()` with a custom title for 5xx errors.
- **`FavoriteSourcesController.cs`** — returns `Problem(...)` for unexpected errors.
- **`ActionResultFromCreateFavoriteSourceResultAssembler.cs`** — maps `UnexpectedError` to a `ProblemDetails` response.

**Example response:**
```json
{
  "type": "about:blank",
  "title": "Unexpected Server Error",
  "status": 500,
  "detail": "An unexpected error occurred while processing your request."
}
```

**Learn more:**
- [RFC 7807 — Problem Details for HTTP APIs](https://datatracker.ietf.org/doc/html/rfc7807)
- [ProblemDetails in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/web-api/problemdetails)

---

### 3.5. Localization with `.resx` Resources

**What it is:** Localization (i18n) allows an application to present user-facing text in different languages. ASP.NET Core uses `.resx` resource files and `IStringLocalizer<T>` for this.

**Why it matters:** It separates text from code, making it easy to add new languages without changing logic.

**Where in the codebase:**

- **`SharedResource.cs`** — a marker class that tells `IStringLocalizer` which `.resx` files to use.
- **`Program.cs`** — configures supported cultures:
  ```csharp
  string[] supportedCultures = ["en", "en-US", "es", "es-PE"];
  ```
- **`FavoriteSourcesController.cs`** — uses `IStringLocalizer<SharedResource>` to localize response messages.
- **`CreateFavoriteSourceResource.cs`** — uses localized error messages in `[Required]` attributes.
- **`LocalizedOpenApiAssembler.cs`** — localizes OpenAPI operation summaries, descriptions, and response texts.

**Learn more:**
- [Localization in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/localization)

---

### 3.6. OpenAPI Generation & Scalar

**What it is:** OpenAPI (formerly Swagger) is a specification for describing REST APIs. ASP.NET Core 10+ has built-in OpenAPI generation. Scalar is a modern, interactive API documentation UI.

**Why it matters:** It auto-generates API documentation that developers can explore and test directly in the browser.

**Where in the codebase:**

- **`Program.cs`** — configures OpenAPI with transformers:
  ```csharp
  builder.Services.AddOpenApi(options =>
  {
      options.AddOperationTransformer(LocalizedOpenApiAssembler.TransformOperationAsync);
      options.AddSchemaTransformer(LocalizedOpenApiAssembler.TransformSchemaAsync);
  });
  ```
- **`Program.cs`** — maps the endpoints:
  ```csharp
  app.MapOpenApi();
  app.MapScalarApiReference();
  ```
- **`LocalizedOpenApiAssembler.cs`** — custom transformers that inject localized text into the OpenAPI document.
- **`FavoriteSourcesController.cs`** — uses `[ProducesResponseType]` attributes to document response types.

**Learn more:**
- [OpenAPI in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/web-api/setup-openapi)
- [Scalar API References](https://scalar.com/)

---

### 3.7. Routing Conventions

**What it is:** ASP.NET Core routing conventions allow you to apply rules to all controllers or actions automatically, instead of configuring each route individually.

**Why it matters:** It enforces a consistent URL style across the entire API without repeating attributes.

**Where in the codebase:**

- **`KebabCaseRouteNamingConvention.cs`** — implements `IControllerModelConvention` to convert `[controller]` tokens to kebab-case.
- **`StringExtensions.cs`** (ASP folder) — provides `ToKebabCase()` using a compiled regex.
- **`Program.cs`** — registers the convention:
  ```csharp
  builder.Services.AddControllers(options =>
      options.Conventions.Add(new KebabCaseRouteNamingConvention()));
  ```

**Result:** `FavoriteSourcesController` with `[Route("api/v1/[controller]")]` becomes `/api/v1/favorite-sources`.

**Learn more:**
- [Routing in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/routing)

---

### 3.8. Structured Logging

**What it is:** Structured logging captures log messages with named properties (placeholders like `{PropertyName}`) instead of string concatenation. This allows log aggregation tools to index and query by property.

**Why it matters:** It makes debugging and monitoring much easier in production. You can filter logs by `NewsApiKey`, `SourceId`, or any other property.

**Where in the codebase:**

- **`FavoriteSourceCommandService.cs`** — uses structured logging extensively:
  ```csharp
  logger.LogWarning("Duplicate favorite source rejected for NewsApiKey {NewsApiKey} and SourceId {SourceId}",
      command.NewsApiKey.MaskedForLogging(), command.SourceId);
  ```
- **`FavoriteSourcesController.cs`** — logs validation failures and unexpected errors.
- **`ILogger<T>`** is injected via primary constructor.

**Learn more:**
- [Logging in .NET](https://learn.microsoft.com/en-us/dotnet/core/extensions/logging)

---

## 4. Entity Framework Core

### 4.1. Code-First Migrations

**What it is:** EF Core Code-First Migrations let you define your database schema in C# code (entity classes and `OnModelCreating`). Migrations then generate the SQL to create or update the database.

**Why it matters:** It keeps the database schema in version control alongside the code, making it reproducible and auditable.

**Where in the codebase:**

- **`Migrations/`** folder — contains generated migration files (`InitialCreate.cs`, `InitialCreate.Designer.cs`, `AppDbContextModelSnapshot.cs`).
- **`AppDbContext.cs`** — defines the model in `OnModelCreating()`:
  ```csharp
  builder.Entity<FavoriteSource>().HasKey(f => f.Id);
  builder.Entity<FavoriteSource>()
      .HasIndex(f => new { f.NewsApiKey, f.SourceId }).IsUnique();
  ```

**Learn more:**
- [EF Core Migrations](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations)

---

### 4.2. Value Converter Mapping

**What it is:** Value Converters in EF Core translate between a .NET type and a database column type. This lets you use custom types (like Value Objects) in your domain while storing them as standard SQL types.

**Why it matters:** It bridges the gap between domain types and database types without exposing the database schema in the domain model.

**Where in the codebase:**

In `AppDbContext.OnModelCreating()`:
```csharp
builder.Entity<FavoriteSource>().Property(f => f.SourceId)
    .HasConversion(
        vo => vo.Value,              // C# → DB: extract the string
        value => new SourceId(value) // DB → C#: construct the value object
    )
    .IsRequired();
```

**Learn more:**
- [EF Core Value Conversions](https://learn.microsoft.com/en-us/ef/core/modeling/value-conversions)

---

### 4.3. Save Changes Interceptors

**What it is:** EF Core Interceptors let you hook into the EF Core pipeline (query execution, save changes, command execution) to run custom logic.

**Why it matters:** It centralizes cross-cutting persistence concerns like auditing, soft deletes, or concurrency checks.

**Where in the codebase:**

- **`AuditableEntityInterceptor.cs`** — implements `SaveChangesInterceptor`:
  - On `SavingChanges`, it finds all `IAuditableEntity` entries and sets `CreatedAt`/`UpdatedAt`.
- **`AppDbContext.OnConfiguring()`** — registers the interceptor:
  ```csharp
  builder.AddInterceptors(new AuditableEntityInterceptor());
  ```

**Learn more:**
- [EF Core Interceptors](https://learn.microsoft.com/en-us/ef/core/logging-diagnostics/interceptors)

---

### 4.4. Snake Case Naming Convention

**What it is:** A naming convention that converts PascalCase C# identifiers to `snake_case` database identifiers (table names, column names, keys, indexes).

**Why it matters:** Many databases (especially MySQL/PostgreSQL) use `snake_case` as the standard naming convention. This keeps the database schema consistent with community norms.

**Where in the codebase:**

- **`ModelBuilderExtensions.cs`** — iterates over all entity types and applies `ToSnakeCase()` and `ToPlural()`:
  ```csharp
  entity.SetTableName(tableName.ToPlural().ToSnakeCase());
  property.SetColumnName(property.GetColumnName().ToSnakeCase());
  ```
- **`StringExtensions.cs`** (EF folder) — provides `ToSnakeCase()` and `ToPlural()` (using Humanizer).
- **`AppDbContext.OnModelCreating()`** — applies the convention:
  ```csharp
  builder.UseSnakeCaseNamingConvention();
  ```

**Result:** `FavoriteSource` entity → `favorite_sources` table, `NewsApiKey` property → `news_api_key` column.

**Learn more:**
- [EF Core Naming Strategies](https://learn.microsoft.com/en-us/ef/core/managing-schemas/naming-strategies)

---

### 4.5. Automatic Migration on Startup

**What it is:** Applying pending EF Core migrations automatically when the application starts, so you don't need to run `dotnet ef database update` manually.

**Why it matters:** It simplifies deployment — the database schema is always up to date when the application runs.

**Where in the codebase:**

In `Program.cs`:
```csharp
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();
}
```

**Learn more:**
- [EF Core EnsureCreated and Migrate](https://learn.microsoft.com/en-us/ef/core/managing-schemas/ensure-created)

---

## 5. C# Language Features

### 5.1. Records

**What it is:** Records (introduced in C# 9) are reference types with built-in value-based equality. They are immutable by default and provide a concise syntax for data carriers.

**Why it matters:** They reduce boilerplate for DTOs, commands, queries, and value objects. The compiler generates `Equals`, `GetHashCode`, and `ToString` automatically.

**Where in the codebase:**

| Type | File | Purpose |
|------|------|---------|
| `CreateFavoriteSourceCommand` | `CreateFavoriteSourceCommand.cs` | Domain command |
| `GetFavoriteSourceByIdQuery` | `GetFavoriteSourceByIdQuery.cs` | Domain query |
| `NewsApiKey` | `NewsApiKey.cs` | Value object |
| `SourceId` | `SourceId.cs` | Value object |
| `CreateFavoriteSourceResource` | `CreateFavoriteSourceResource.cs` | REST input DTO |
| `FavoriteSourceResource` | `FavoriteSourceResource.cs` | REST output DTO |
| `Result<TValue, TError>` | `Result.cs` | Result pattern (abstract record with sealed subtypes) |

**Learn more:**
- [Records in C#](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/record)

---

### 5.2. Primary Constructors

**What it is:** Primary constructors (enhanced in C# 12) let you declare constructor parameters directly on the class, struct, or record declaration, eliminating the need for a separate constructor body.

**Why it matters:** It dramatically reduces boilerplate, especially for dependency injection and simple data carriers.

**Where in the codebase:**

- **Controllers:**
  ```csharp
  public class FavoriteSourcesController(
      IFavoriteSourceCommandService favoriteSourceCommandService,
      IFavoriteSourceQueryService favoriteSourceQueryService,
      IStringLocalizer<SharedResource> localizer,
      ILogger<FavoriteSourcesController> logger) : ControllerBase
  ```
- **Services:**
  ```csharp
  public class FavoriteSourceCommandService(
      IFavoriteSourceRepository favoriteSourceRepository,
      IUnitOfWork unitOfWork,
      ILogger<FavoriteSourceCommandService> logger) : IFavoriteSourceCommandService
  ```
- **Repositories:**
  ```csharp
  public class FavoriteSourceRepository(AppDbContext context)
      : BaseRepository<FavoriteSource>(context), IFavoriteSourceRepository
  ```
- **DbContext:**
  ```csharp
  public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
  ```

**Learn more:**
- [Primary Constructors (C# 12)](https://learn.microsoft.com/en-us/dotnet/csharp/minimal-apis/primary-constructors)

---

### 5.3. Pattern Matching

**What it is:** Pattern matching allows you to test values against patterns and extract data in a single expression. C# has evolved this feature extensively (type patterns, property patterns, `switch` expressions, `when` guards).

**Why it matters:** It replaces verbose `if-else` chains and `switch` statements with concise, readable expressions.

**Where in the codebase:**

- **`Result.cs`** — uses `switch` expressions:
  ```csharp
  public bool IsSuccess => this is Success;
  public bool IsFailure => this is Failure;
  ```
- **`ActionResultFromCreateFavoriteSourceResultAssembler.cs`** — nested `switch` expressions:
  ```csharp
  result switch
  {
      Result<...>.Success success => controller.CreatedAtAction(...),
      Result<...>.Failure failure => failure.Error switch
      {
          ECreateFavoriteSourceError.DuplicateFavoriteSource => controller.Conflict(...),
          ECreateFavoriteSourceError.UnexpectedError => controller.Problem(...),
          _ => controller.Problem(...)
      },
      _ => controller.Problem(...)
  }
  ```
- **`FavoriteSourceCommandService.cs`** — null pattern:
  ```csharp
  if (existingSource is not null) { ... }
  ```
- **`AuditableEntityInterceptor.cs`** — constant patterns:
  ```csharp
  if (entry.State is EntityState.Added or EntityState.Modified)
  ```

**Learn more:**
- [Pattern Matching in C#](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/functional/pattern-matching)

---

### 5.4. Nullable Reference Types

**What it is:** Nullable Reference Types (NRT, C# 8) let the compiler track which reference types can be `null` and which cannot. Enabled via `<Nullable>enable</Nullable>` in the project file.

**Why it matters:** It catches null reference errors at compile time instead of runtime, reducing `NullReferenceException` bugs.

**Where in the codebase:**

- **`CatchUpPlatform.API.csproj`** — enables NRT:
  ```xml
  <Nullable>enable</Nullable>
  ```
- **Throughout the codebase:** `FavoriteSource?` (nullable), `string` (non-nullable), `string?` (nullable string).
- **`FavoriteSource.cs`** — uses `null!` for EF Core parameterless constructor:
  ```csharp
  public FavoriteSource()
  {
      NewsApiKey = null!;
      SourceId = null!;
  }
  ```

**Learn more:**
- [Nullable Reference Types](https://learn.microsoft.com/en-us/dotnet/csharp/nullable-references)

---

### 5.5. `using` Directives (Implicit Usings)

**What it is:** Implicit Usings (C# 10) automatically import common namespaces (`System`, `System.Collections.Generic`, `System.Linq`, etc.) without explicit `using` directives.

**Why it matters:** It reduces boilerplate at the top of every file.

**Where in the codebase:**

- **`CatchUpPlatform.API.csproj`** — enables implicit usings:
  ```xml
  <ImplicitUsings>enable</ImplicitUsings>
  ```

**Learn more:**
- [Implicit Usings](https://learn.microsoft.com/en-us/dotnet/core/project-sdk/implicit-usings)

---

### 5.6. Source Generators (`[GeneratedRegex]`)

**What it is:** Source Generators (C# 9+) run at compile time and generate additional C# code. `[GeneratedRegex]` (C# 11) is a built-in source generator that creates an optimized `Regex` instance at compile time.

**Why it matters:** It moves regex compilation from runtime to compile time, improving performance and catching regex errors early.

**Where in the codebase:**

- **`StringExtensions.cs`** (ASP folder):
  ```csharp
  [GeneratedRegex("(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])", RegexOptions.Compiled)]
  private static partial Regex KebabCaseRegex();
  ```

**Learn more:**
- [Source Generators Overview](https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview)
- [GeneratedRegex Attribute](https://learn.microsoft.com/en-us/dotnet/api/system.codeanalysis.generatedregexattribute)

---

### 5.7. `when` Exception Filters

**What it is:** Exception filters (`catch ... when (...)`) let you conditionally catch an exception based on a predicate, without entering the catch block if the condition is false.

**Why it matters:** It provides fine-grained control over exception handling without nested `if` statements inside catch blocks.

**Where in the codebase:**

- **`FavoriteSourceCommandService.cs`**:
  ```csharp
  catch (DbUpdateException ex) when (IsDuplicateKeyViolation(ex))
  {
      // Handle duplicate key specifically
  }
  catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
  {
      throw; // Re-throw cancellation, don't swallow it
  }
  ```

**Learn more:**
- [Exception Filters (C#)](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/try-catch#filter)

---

### 5.8. String Slicing (`^4..`)

**What it is:** C# 8 introduced range and index operators. `^4` means "4 from the end" and `..` creates a range. `value[^4..]` extracts the last 4 characters.

**Why it matters:** It provides a clean, readable way to work with substrings without calculating indices manually.

**Where in the codebase:**

- **`NewsApiKey.cs`**:
  ```csharp
  public static string MaskedForLogging(string? value) =>
      string.IsNullOrEmpty(value)
          ? string.Empty
          : value.Length <= 4
              ? new string('*', value.Length)
              : new string('*', value.Length - 4) + value[^4..];
  ```

**Learn more:**
- [Ranges and Indices in C#](https://learn.microsoft.com/en-us/dotnet/csharp/tutorials/ranges-patterns)

---

### 5.9. `sealed` Classes and Records

**What it is:** The `sealed` modifier prevents a class or record from being inherited.

**Why it matters:** It signals that a type is complete and shouldn't be extended. It also allows the compiler and JIT to optimize virtual calls.

**Where in the codebase:**

- `NewsApiKey` — `public sealed record NewsApiKey`
- `SourceId` — `public sealed record SourceId`
- `Result.Success` and `Result.Failure` — `public sealed record Success(...)` and `public sealed record Failure(...)`
- `AuditableEntityInterceptor` — `public sealed class AuditableEntityInterceptor`

**Learn more:**
- [sealed Modifier](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/sealed)

---

### 5.10. `private set` Properties

**What it is:** A property with `private set` can only be modified from within the class itself, not from outside.

**Why it matters:** It enforces encapsulation — the aggregate root controls its own state, preventing external code from corrupting invariants.

**Where in the codebase:**

- **`FavoriteSource.cs`**:
  ```csharp
  public int Id { get; private set; }
  public NewsApiKey NewsApiKey { get; private set; }
  public SourceId SourceId { get; private set; }
  ```

**Learn more:**
- [Auto-Implemented Properties](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/auto-implemented-properties)

---

### 5.11. Nested Types with Abstract Base Record

**What it is:** An abstract record can define nested sealed records that inherit from it, creating a discriminated union-like structure.

**Why it matters:** It creates a type-safe way to represent one-of-many outcomes without using exceptions or magic values.

**Where in the codebase:**

- **`Result.cs`**:
  ```csharp
  public abstract record Result<TValue, TError>
  {
      public sealed record Success(TValue Value) : Result<TValue, TError>;
      public sealed record Failure(TError Error) : Result<TValue, TError>;
  }
  ```

This pattern is the foundation of the Result/Either type used throughout the application.

---

### 5.12. `IEnumerable<char>` with Local Functions

**What it is:** A local function is a function defined inside another method. Combined with `IEnumerable<char>` and `yield return`, it creates a lazy character iterator.

**Why it matters:** It keeps helper logic encapsulated within the method that uses it, avoiding separate static methods or classes.

**Where in the codebase:**

- **`StringExtensions.cs`** (EF folder):
  ```csharp
  public static string ToSnakeCase(this string text)
  {
      return new string(Convert(text.GetEnumerator()).ToArray());

      static IEnumerable<char> Convert(CharEnumerator e)
      {
          if (!e.MoveNext()) yield break;
          yield return char.ToLower(e.Current);
          while (e.MoveNext())
              if (char.IsUpper(e.Current))
              {
                  yield return '_';
                  yield return char.ToLower(e.Current);
              }
              else
              {
                  yield return e.Current;
              }
      }
  }
  ```

**Learn more:**
- [Local Functions in C#](https://learn.microsoft.com/en-us/dotnet/csharp/methods#local-functions)

---

## 6. DevOps & Containerization

### 6.1. Multi-Stage Dockerfile

**What it is:** A multi-stage Docker build uses multiple `FROM` statements to create a smaller final image. The build stage compiles the application, and the runtime stage only includes the published output.

**Why it matters:** It produces a smaller, more secure image by excluding the SDK, source code, and build tools from the final image.

**Where in the codebase:**

- **`Dockerfile`**:
  ```dockerfile
  # Stage 1: Build
  FROM mcr.microsoft.com/dotnet/sdk:10.0 AS builder
  WORKDIR /app
  COPY CatchUpPlatform.API/*.csproj CatchUpPlatform.API/
  RUN dotnet restore ./CatchUpPlatform.API
  COPY . .
  RUN dotnet publish ./CatchUpPlatform.API -c Release -o out

  # Stage 2: Runtime
  FROM mcr.microsoft.com/dotnet/aspnet:10.0
  WORKDIR /app
  COPY --from=builder /app/out .
  EXPOSE 80
  ENTRYPOINT ["dotnet", "CatchUpPlatform.API.dll"]
  ```

**Key optimization:** The `COPY *.csproj` + `dotnet restore` before `COPY . .` leverages Docker layer caching — dependency restoration is skipped if only source code changes.

**Learn more:**
- [Multi-Stage Builds — Docker Docs](https://docs.docker.com/build/building/multi-stage/)
- [.NET Docker Images](https://github.com/dotnet/dotnet-docker)

---

### 6.2. Environment Variable Expansion

**What it is:** Using environment variables in configuration files and expanding them at runtime, so secrets are never hardcoded.

**Why it matters:** It keeps sensitive data (passwords, connection strings) out of the codebase and allows different configurations per environment.

**Where in the codebase:**

- **`Program.cs`**:
  ```csharp
  var connectionString = Environment.ExpandEnvironmentVariables(connectionStringTemplate);
  ```
- **`README.md`** documents production config:
  ```
  %DATABASE_URL%
  %DATABASE_USER%
  %DATABASE_PASSWORD%
  %DATABASE_SCHEMA%
  ```

**Learn more:**
- [ASP.NET Core Configuration](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration)

---

## 7. Recommended Learning Path

If you're new to these concepts, follow this order to build your understanding progressively:

### Phase 1: Foundations
1. **C# Language Basics** — records, pattern matching, nullable reference types
2. **ASP.NET Core Fundamentals** — minimal hosting, dependency injection, model binding
3. **REST API Design** — HTTP methods, status codes, resource naming

### Phase 2: Architecture
4. **Layered Architecture** — separate concerns into layers
5. **Repository Pattern** — abstract data access
6. **Entity Framework Core** — ORM basics, migrations, LINQ

### Phase 3: Advanced Patterns
7. **Domain-Driven Design** — bounded contexts, aggregates, value objects
8. **CQS** — separate commands from queries
9. **Result Pattern** — handle expected failures without exceptions
10. **Unit of Work** — atomic transactions

### Phase 4: Production Readiness
11. **Localization** — multi-language support
12. **OpenAPI / Scalar** — API documentation
13. **Structured Logging** — observability
14. **Docker** — containerization
15. **ProblemDetails** — standardized error responses

---

## 8. External References

### Books
- **"Domain-Driven Design: Tackling Complexity in the Heart of Software"** — Eric Evans
- **"Clean Architecture: A Craftsman's Guide to Software Structure and Design"** — Robert C. Martin
- **"C# in Depth"** — Jon Skeet

### Official Documentation
- [Microsoft .NET Docs](https://learn.microsoft.com/en-us/dotnet/)
- [ASP.NET Core Docs](https://learn.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core Docs](https://learn.microsoft.com/en-us/ef/core/)
- [C# Language Reference](https://learn.microsoft.com/en-us/dotnet/csharp/)

### Articles & Blogs
- [Clean Architecture — Uncle Bob](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [DDD Bounded Contexts — Martin Fowler](https://martinfowler.com/bliki/BoundedContext.html)
- [Railway-Oriented Programming — Scott Wlaschin](https://fsharpforfunandprofit.com/posts/recipe-part2/)
- [Aggregates in DDD — Vlad Khorikov](https://enterprisecraftsmanship.com/posts/ddd-aggregates-ensure-consistency-available-updates/)

### Specifications
- [RFC 7807 — Problem Details for HTTP APIs](https://datatracker.ietf.org/doc/html/rfc7807)
- [OpenAPI Specification 3.1](https://spec.openapis.org/oas/v3.1.0)

### Tools
- [Scalar API References](https://scalar.com/)
- [Humanizer Library](https://github.com/Humanizr/Humanizer)
- [Docker Docs](https://docs.docker.com/)

---

> **Tip:** The best way to learn from this codebase is to read the [Architecture Decision Records (ADRs)](./adrs.md) alongside this guide. Each ADR explains the *why* behind a specific design choice, which is often more valuable than the *how*.
