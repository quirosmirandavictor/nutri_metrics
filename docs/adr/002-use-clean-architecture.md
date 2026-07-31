# ADR-002: Use clean architecture


## Status

Accepted <!-- Proposed | Accepted | Rejected | Deprecated | Superseded -->


## Date

2026-07-20

## Context

Nutri Metrics is a light API that use natural language in spanish to get your daily food calories,
This type of system requires strong relationship with the domain, in this case the user foods.

This API work with two specific integrations, Free google translation API service and calorieNinjas API,
but in the future, we could use a different providers for translation or calories tracking.

Additionaly, Nutri Metrics works with MySQL data base but, same at external APIS integration
the database motor  could need more procesing capabilities and scalability features that MySQL could not cover.

## Decision

We will use clean architecture. Business rules will reside in the domain and application layer, independient of frameworks,
databases and external technologies.

The Dependencies always point inward and frameworks, persistence mechanisms and external services will be treated as implementation details through adapters and interfaces.

## Alternatives Considered

### Option A: Clean Architecture (selected)

**Pros**

- **Framework independence:** Business rules remain isolated from frameworks, databases, and external technologies.
- **High maintainability and testability:** Domain and application layers can be tested without infrastructure dependencies.
- **Supports modular evolution:** Well-defined boundaries allow modules to evolve independently and simplify future extraction into microservices.

**Cons**

- Higher initial complexity due to additional layers and abstractions.
- Requires disciplined enforcement of architectural boundaries across the development team.
- More boilerplate than framework-centric approaches, especially for smaller features.

### Option B: Traditional Layered Architecture (N-Layer)

**Pros**

- **Simple and familiar:** A widely adopted architecture that is easy for most developers to understand.
- **Clear technical separation:** Presentation, business, and data access responsibilities are organized into distinct layers.
- **Suitable for CRUD applications:** Works well for applications with relatively simple business rules.

**Cons**

- **Business logic depends on infrastructure:** Domain services often become tightly coupled to persistence and framework implementations.
- **Limited flexibility:** Replacing databases or external technologies typically requires changes across multiple layers.
- **Weaker domain isolation:** Layer boundaries alone do not guarantee independence of business rules or low coupling between modules.

### Option C: Hexagonal Architecture (Ports & Adapters)

**Pros**

- **Framework independence:** The domain core communicates only through ports (interfaces), keeping it isolated from frameworks, databases, and external technologies — conceptually very close to Clean Architecture's goals.
- **Explicit boundaries via ports and adapters:** Each integration point (database, external API, messaging) is modeled as an adapter implementing a port, making it easy to swap implementations (e.g., replacing CalorieNinjas with another provider) without touching the domain.
- **High testability:** The domain can be tested in isolation by mocking ports, without needing infrastructure dependencies.

**Cons**

- **Conceptual overlap with Clean Architecture:** Distinguishing "ports" from application interfaces and "adapters" from infrastructure implementations can create ambiguity and inconsistent conventions across the team.
- **Less prescriptive layering:** Unlike Clean Architecture's concentric layers (Domain, Application, Infrastructure, Presentation), Hexagonal Architecture doesn't define an internal structure for the "inside" of the hexagon, which can lead to inconsistent organization within the core as the system grows.
- **Similar boilerplate cost:** Requires nearly the same number of abstractions (interfaces per integration) as Clean Architecture, without providing additional benefits for a modular monolith already organized by bounded modules.

### Positive

- **Framework independence:** Business rules remain isolated from Framework, the database, and external technologies, reducing vendor lock-in.
- **Improved maintainability:** Clear separation of responsibilities makes the codebase easier to understand, modify, and extend.
- **Higher testability:** Domain and application logic can be unit tested without relying on infrastructure components.
- **Supports modular evolution:** Explicit module boundaries simplify future extraction into independent services if required.

### Negative / Trade-offs

- **Higher initial complexity:** Additional layers, interfaces, and dependency inversion introduce more architectural overhead.
- **More boilerplate code:** Simple features may require additional abstractions compared to framework-centric approaches.
- **Requires architectural discipline:** The team must consistently enforce dependency rules and module boundaries to prevent architectural erosion.

### Items to Monitor

- **Boundary violations:** Ensure dependencies always point inward and modules do not access each other's internal implementations.
- **Architecture consistency:** Verify that new features follow the established Clean Architecture structure instead of introducing shortcuts.
- **Testing coverage:** Monitor the proportion of unit tests focused on the domain and application layers to ensure business logic remains independent.
## References

- ADR-001: Adopt a monolith architecture
- Robert C. Martin — *Clean Architecture: A Craftsman's Guide to Software Structure and Design*
- Martin Fowler — *Patterns of Enterprise Application Architecture*