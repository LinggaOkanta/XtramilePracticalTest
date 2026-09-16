# Practical Test Xtramile — Weather & Notes Application

A production-grade **ASP.NET Core (.NET 10) Web API** and lightweight, responsive **front-end web client** built according to **Onion Architecture** and **CQRS (Command Query Responsibility Segregation)** principles.

The application enables users to select countries and cities via cascading dropdowns, view real-time weather metrics proxied from OpenWeatherMap (with domain-level Fahrenheit to Celsius conversions), and save custom weather notes or favorite cities with state persistence via **Entity Framework Core**.

> Architectural specifications, design rules, and AI assistant guidelines are detailed in [`GEMINI.md`](GEMINI.md).

---

## Tech Stack

- **Framework**: .NET 10 Web API, C# 13
- **Architecture**: Onion Architecture (Domain, Application, Infrastructure, Presentation)
- **CQRS Pattern**: MediatR with pipeline behaviors (`ValidationBehavior`, `LoggingBehavior`)
- **Validation**: FluentValidation
- **Persistence**: Entity Framework Core (SQLite / In-Memory provider) with read optimization (`.AsNoTracking()`)
- **External Integration**: OpenWeatherMap API proxy behind `IWeatherService` abstraction with resilient offline `MockWeatherService`
- **Front-End Client**: Lightweight, modern responsive UI (HTML5, Vanilla CSS & JS) served via `wwwroot`
- **API Documentation**: OpenAPI / Swagger UI
- **Testing**: xUnit, FluentAssertions, Moq, and `Microsoft.AspNetCore.Mvc.Testing` (**100% offline, zero live network calls**)

---

## Architecture Overview (Onion Architecture + CQRS)

The solution decouples domain logic, application use cases, infrastructure adapters, and API endpoints along concentric rings:

```
                  ┌──────────────────────────────────────────────┐
                  │                 Presentation                 │
                  │       (Controllers / Middleware / UI)        │
                  └──────────────────────┬───────────────────────┘
                                         ▼
                  ┌──────────────────────────────────────────────┐
                  │                 Application                  │
                  │  (CQRS Commands/Queries, Handlers, DTOs,     │
                  │   Validation Behaviors, Service Interfaces)  │
                  └──────────────────────┬───────────────────────┘
                                         ▼
                  ┌──────────────────────────────────────────────┐
                  │                    Domain                    │
                  │   (Entities, Value Objects, Domain Rules,    │
                  │    Pure TemperatureConverter Logic)          │
                  └──────────────────────────────────────────────┘
                                         ▲
                  ┌──────────────────────┴───────────────────────┐
                  │                Infrastructure                │
                  │   (EF Core DbContext, OpenWeatherMap Client, │
                  │    Mock Weather Service, Data Seeders)       │
                  └──────────────────────────────────────────────┘
```

### Layer Boundaries & Separation of Concerns

1. **Domain Layer**:
   - Zero framework dependencies (no EF Core, no ASP.NET, no MediatR).
   - Contains core entities (`Country`, `City`, `WeatherNote`, `FavoriteCity`) and Value Objects (`Temperature`, `WindInfo`).
   - Encapsulates pure domain calculation: `TemperatureConverter` ($^\circ\text{F} \rightarrow ^\circ\text{C}$ conversion and precision rounding).
2. **Application Layer**:
   - Implements CQRS via **MediatR**: Request/Query/Command definitions and corresponding handlers.
   - Enforces validation using **FluentValidation** through an automatic MediatR pipeline behavior (`ValidationBehavior`).
   - Defines contracts & abstractions: `IApplicationDbContext`, `IWeatherService`, `IDateTimeProvider`.
3. **Infrastructure Layer**:
   - Implements data persistence with `ApplicationDbContext` (SQLite / In-Memory).
   - Pre-seeds master countries and cities (`MasterDataSeeder`) so dropdowns work out of the box.
   - Implements `OpenWeatherMapService` using `IHttpClientFactory` and `MockWeatherService` for deterministic offline execution.
4. **Presentation / API Layer**:
   - Thin ASP.NET Core controllers routing requests directly through `ISender.Send()`.
   - Global exception handling middleware rendering RFC 9110 compliant `ProblemDetails` / `ValidationProblemDetails`.
   - Serves static front-end assets from `wwwroot/`.

---

## Solution Layout

```
Practical Test Xtramile/
├── Practical Test Xtramile.slnx               # Solution file
├── GEMINI.md                                  # Architectural plan & detailed blueprint
├── README.md                                  # This documentation file
│
├── src/
│   └── Practical_Test_Xtramile/               # Web API Host & Onion Layers
│       ├── Domain/                            # Entities, Value Objects, Rules
│       │   ├── Entities/                      # Country, City, WeatherNote, FavoriteCity
│       │   ├── ValueObjects/                  # Temperature, WindInfo
│       │   └── Services/                      # TemperatureConverter (Pure Domain)
│       ├── Application/                       # CQRS Commands/Queries, Handlers, DTOs
│       │   ├── Common/                        # Behaviors, Exceptions, Interfaces
│       │   └── Features/                      # Countries and Weather features
│       ├── Infrastructure/                    # EF Core, External HTTP, Seeders
│       │   ├── Persistence/                   # DbContext, Configurations, Seeding
│       │   └── WeatherApi/                    # OpenWeatherMapService & MockWeatherService
│       ├── Presentation/                      # Controllers & Middleware
│       │   ├── Controllers/                   # CountriesController, WeatherController
│       │   └── Middleware/                    # ExceptionHandlingMiddleware
│       └── wwwroot/                           # Front-End Web Client
│           ├── index.html                     # Responsive UI layout
│           ├── css/styles.css                 # Curated design tokens & CSS
│           └── js/app.js                      # Cascading dropdowns, fetch & actions
│
└── tests/
    └── Practical_Test_Xtramile.Tests/         # Automated Test Suite (100% Offline)
        ├── UnitTests/                         # Domain (edge cases) & CQRS Handler tests
        └── IntegrationTests/                  # WebApplicationFactory & MockHttpMessageHandler
```

---

## API Workflows & Feature Set

### Read Endpoints (Optimized with `.AsNoTracking()`)

| Method | Endpoint | Description |
| :--- | :--- | :--- |
| `GET` | `/api/countries` | Returns all available countries (`CountryDto`) |
| `GET` | `/api/countries/{countryCode}/cities` | Returns cities filtered by country code (`CityDto`) |
| `GET` | `/api/weather/{cityName}` | Proxies OpenWeatherMap via `IWeatherService`, converts Fahrenheit to Celsius, and returns full metrics |

### Write Endpoints (State Persistence via EF Core)

| Method | Endpoint | Description | Status Code |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/weather/notes` | Dispatches MediatR command to persist a custom note/log for city weather | `201 Created` |
| `POST` | `/api/cities/favorites` | Dispatches MediatR command to save a city to user favorites | `201 Created` |

### Complete Weather Payload Structure

```json
{
  "location": {
    "city": "Melbourne",
    "country": "Australia",
    "countryCode": "AU"
  },
  "timeUtc": "2026-09-16T02:45:00Z",
  "wind": {
    "speedMph": 12.5,
    "directionDegrees": 180,
    "directionCardinal": "S"
  },
  "visibilityMeters": 10000,
  "pressureHpa": 1013.25,
  "skyConditions": "Clear",
  "temperature": {
    "fahrenheit": 68.0,
    "celsius": 20.0
  },
  "dewPoint": {
    "fahrenheit": 50.0,
    "celsius": 10.0
  },
  "relativeHumidityPercent": 55
}
```

---

## Domain Business Logic: Temperature Conversion

External weather telemetry in imperial units arrives in **Fahrenheit**. The domain model converts this value to **Celsius**:

$$C = (F - 32) \times \frac{5}{9}$$

- **Rounding**: Rounded to 2 decimal places using `Math.Round(val, 2, MidpointRounding.AwayFromZero)`.
- **Edge Cases Tested**:
  - Boiling Point: $212.0^\circ\text{F} \rightarrow 100.0^\circ\text{C}$
  - Freezing Point: $32.0^\circ\text{F} \rightarrow 0.0^\circ\text{C}$
  - Crossover Point: $-40.0^\circ\text{F} \rightarrow -40.0^\circ\text{C}$
  - Absolute Zero: $-459.67^\circ\text{F} \rightarrow -273.15^\circ\text{C}$
  - Zero Fahrenheit: $0.0^\circ\text{F} \rightarrow -17.78^\circ\text{C}$

---

## Front-End Application Workflow

The web client in `wwwroot/` provides an intuitive user experience:
1. **Country Selection**: Fetches `/api/countries` and populates the Country dropdown.
2. **City Selection**: Once a country is selected, enables the City dropdown and fetches `/api/countries/{code}/cities`.
3. **Weather Metric Display**: Selecting a city queries `/api/weather/{cityName}` and renders a comprehensive weather card with dual temperatures ($^\circ\text{C}$ and $^\circ\text{F}$), wind, sky condition, humidity, pressure, and dew point.
4. **Interactive Action**: Includes form fields to enter a custom weather note or save the city to favorites, dispatching `POST /api/weather/notes` with immediate UI feedback (toast/notification).

---

## Automated Testing Strategy (100% Offline Constraint)

All automated tests are guaranteed to run **completely offline with zero live network calls**:

1. **Domain Logic Tests**:
   - Unit test `TemperatureConverter` across edge cases (boiling, freezing, negative, precision rounding).
2. **CQRS Handler Tests**:
   - `GetWeatherByCityQueryHandler`: Tested in isolation with a mocked `IWeatherService`.
   - `CreateWeatherNoteCommandHandler`: Verifies EF Core state persistence and ID generation against an In-Memory DbContext.
3. **API Integration Tests**:
   - Uses `WebApplicationFactory<Program>`.
   - Intercepts outbound HTTP calls with `MockHttpMessageHandler` or registers `MockWeatherService`.
   - Tests end-to-end API response codes (`200 OK`, `201 Created`, `400 Bad Request`, `404 Not Found`).

---

## Prerequisites & Configuration

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)

### Configuration (`appsettings.json`)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=App_Data/weather.db"
  },
  "OpenWeatherMap": {
    "ApiKey": "YOUR_OPENWEATHERMAP_API_KEY",
    "BaseUrl": "https://api.openweathermap.org/data/2.5/",
    "UseMock": false
  }
}
```

> **Offline Mode**: Setting `"OpenWeatherMap:UseMock": true` switches weather retrieval to `MockWeatherService`, allowing complete local exploration without an active API key.

---

## Build, Run, and Test Instructions

### 1. Build Solution
```bash
dotnet build "Practical Test Xtramile.slnx"
```

### 2. Run Web API & Front-End
```bash
dotnet run --project "Practical Test Xtramile/Practical Test Xtramile.csproj"
```

- **Web Client UI**: `http://localhost:5000/index.html` (or `https://localhost:5001/index.html`)
- **Interactive Swagger UI**: `http://localhost:5000/swagger`
- **OpenAPI Document**: `http://localhost:5000/openapi/v1.json`

### 3. Run Automated Tests
```bash
dotnet test "tests/Practical_Test_Xtramile.Tests/Practical_Test_Xtramile.Tests.csproj"
```

---

## Deliverables & Repository Structure (Point 6 Compliance)

This repository contains all required deliverables in a single unified solution:
- **Backend**: ASP.NET Core Web API located under `src/Practical_Test_Xtramile/` structured via Onion Architecture and MediatR CQRS.
- **Frontend**: Lightweight, responsive web client hosted directly from `src/Practical_Test_Xtramile/wwwroot/`.
- **Automated Tests**: Comprehensive test suite under `tests/Practical_Test_Xtramile.Tests/` containing domain, handler, and offline integration tests.
- **AI Tooling & Configuration**: Project-level AI guidance file (`GEMINI.md`) located at the solution root.

---

## EF Core Configuration & State Management

### 1. Provider & DbContext Setup
- **Configured Providers**:
  - **SQLite** (`Microsoft.EntityFrameworkCore.Sqlite`): Persisted local database configured via `ConnectionStrings:DefaultConnection` (e.g., `Data Source=App_Data/weather.db`).
  - **In-Memory** (`Microsoft.EntityFrameworkCore.InMemory`): Used during integration and unit testing for deterministic, ephemeral test isolation without disk side-effects.
- **Entity Configurations**: Explicit mapping using `IEntityTypeConfiguration<T>` in `Infrastructure/Persistence/Configurations/`, defining primary keys, column constraints, and foreign key relationships.

### 2. State Mutation & Tracking Rules
- **Write Side (Commands)**:
  - Command handlers inject `IApplicationDbContext`, mutate domain entities, and commit changes via `SaveChangesAsync(cancellationToken)`.
  - State persistence generates a new primary identifier (`Guid`), and the API returns a `201 Created` response with the created resource and `CreatedAtUtc` timestamp.
- **Read Side (Queries - Read Optimization)**:
  - **Mandatory**: All query handlers executing queries against EF Core explicitly use `.AsNoTracking()`.
  - This prevents EF Core from snapshotting and allocating change tracker resources, maximizing read performance and throughput.

### 3. Master Data Seeding & Environment Gating
- `MasterDataSeeder` populates default countries (e.g., Australia, United States, Indonesia, United Kingdom, Japan) and their major cities when the database is empty.
- **Environment & Configuration Validation**:
  - Seeding is **gated** by the configuration flag `Database:SeedData` and environment checks:
    - **Development & Testing**: Defaults to `true` so dropdowns work out-of-the-box locally and during offline integration tests.
    - **Production**: Defaults to `false` in `appsettings.json` (a DBA or idempotent migration script controls production database state, preventing horizontal race conditions across multiple instances).
  - Implementation in `Program.cs`:
    ```csharp
    bool shouldSeed = config.GetValue<bool>(
        "Database:SeedData", 
        defaultValue: env.IsDevelopment() || env.IsEnvironment("Testing")
    );
    if (shouldSeed)
    {
        await MasterDataSeeder.SeedAsync(context);
    }
    ```

---

## Key Design Decisions

1. **Onion Architecture via Folder Separation**:
   - As encouraged in the assessment specification (`"use folder instead of project if you AI agent"`), the project uses cleanly separated folders (`Domain`, `Application`, `Infrastructure`, `Presentation`) within the primary project.
   - This prevents multi-project overhead and slow build times while rigorously enforcing Clean Architecture boundary rules: `Domain` has zero framework dependencies, and `Application` depends only on `Domain`.
2. **CQRS Decoupling with MediatR**:
   - Controllers remain thin and maintain zero business logic. They receive HTTP requests and dispatch an `IRequest<TResponse>` via `ISender`.
   - Cross-cutting concerns like validation (`FluentValidation`) and logging are encapsulated into MediatR pipeline behaviors (`ValidationBehavior`, `LoggingBehavior`) instead of cluttering controller actions.
3. **Pure Domain Temperature Conversion**:
   - The formula $C = (F - 32) \times \frac{5}{9}$ and rounding logic (`Math.Round(..., 2, MidpointRounding.AwayFromZero)`) are placed inside `Domain/Services/TemperatureConverter.cs` rather than the API or database layer.
   - This ensures unit testability without framework dependencies across all edge cases (boiling, freezing, sub-zero, negative crossover).
4. **Resilient Weather Abstraction & 100% Offline Test Guarantee**:
   - `IWeatherService` decouples OpenWeatherMap from the application.
   - For automated tests and environments without internet access, `MockHttpMessageHandler` and `MockWeatherService` provide deterministic responses, guaranteeing **zero live network calls** during `dotnet test`.

---

## AI Prompt File Configuration (Point 5 & 6 Compliance)

To satisfy **Section 5 and Section 6 of the assessment requirements**, this project includes comprehensive AI assistant instructions at the solution root:

- **`GEMINI.md`**: The project-level AI assistant configuration and architectural implementation blueprint defining:
  - Canonical architectural blueprint outlining domain entities, MediatR CQRS workflows, API specifications, and phased roadmap.
  - Strict layer boundary constraints (Onion Architecture: zero framework dependencies in Domain).
  - CQRS and handler conventions (single-responsibility handlers, `.AsNoTracking()` for queries).
  - Code formatting, naming conventions, and common build/test commands.
- **Benefits**: Guarantees that AI-assisted iterations, code reviews, and pair programming adhere strictly to the established architectural standards without layer leaks or regressions.

