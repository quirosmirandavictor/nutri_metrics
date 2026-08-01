# ADR-003: Adopt CQRS (Command Query Responsibility Segregation)

## Status

Accepted <!-- Proposed | Accepted | Rejected | Deprecated | Superseded -->

## Date

2026-07-21

## Context

Nutri Metrics currently exposes a relatively simple CRUD API for tracking nutritional information and user meals.

Although the current use cases are simple, the platform is expected to evolve beyond basic CRUD operations. Future capabilities may include weight tracking, BMI history, body fat percentage, visceral fat measurements, progress dashboards, historical reports, and additional health metrics.

These future features will introduce increasingly complex read models while write operations remain focused on validating and storing domain information.

Since ADR-002 established Clean Architecture as the project's architectural foundation, separating commands from queries naturally complements the goal of keeping the domain independent from persistence concerns and query-specific requirements.

## Decision

We will adopt Command Query Responsibility Segregation (CQRS).

Commands will represent operations that modify the system state and will encapsulate business rules through the application layer.

Queries will retrieve data through dedicated query handlers and optimized read models without introducing read-specific concerns into the domain model.

This separation allows read and write models to evolve independently while preserving the dependency rules established by Clean Architecture.

## Alternatives Considered

### Option A: CQRS (selected)

**Pros**

- **Clear separation of responsibilities:** Read and write operations evolve independently.
- **Protects the domain model:** Query-specific optimizations do not leak into business logic.
- **Supports future growth:** New reporting, dashboards, and analytics can be implemented without affecting command handlers.
- **Aligns with Clean Architecture:** Maintains the domain isolated from infrastructure and read-model concerns.

**Cons**

- Introduces additional complexity compared to a traditional CRUD approach.
- Requires more classes (commands, queries, handlers, DTOs).
- Can be unnecessary for small applications with simple requirements.

### Option B: Traditional CRUD

**Pros**

- **Simple implementation:** Fewer abstractions and less code.
- **Lower development cost:** Faster to implement for small systems.
- **Easy to understand:** Familiar pattern for most developers.

**Cons**

- Read and write concerns become coupled.
- Complex queries tend to leak into application services or repositories.
- Harder to evolve as reporting requirements increase.

### Option C: Application Services with a Shared Domain Model

**Pros**

- **Moderate complexity:** Simpler than CQRS while remaining compatible with Clean Architecture.
- **Reduced boilerplate:** Fewer handlers and DTOs.
- **Suitable for medium-sized applications.**

**Cons**

- Services gradually accumulate both command and query responsibilities.
- Query optimizations can begin to influence the domain model.
- Scaling or evolving read operations independently becomes more difficult.

## Positive

- **Better separation of concerns:** Commands and queries have independent responsibilities.
- **Future-ready architecture:** Supports planned health metrics and analytical features.
- **Improved maintainability:** Read models can evolve without affecting business rules.
- **Keeps the domain clean:** Reporting and projection requirements remain outside the core domain.

## Negative / Trade-offs

- **Higher architectural complexity:** More files, handlers, and abstractions.
- **Additional boilerplate:** Every feature requires command/query definitions and handlers.
- **Potential overengineering:** The current application could function correctly using a simpler CRUD architecture.

## Items to Monitor

- **Complexity vs. value:** Ensure CQRS continues to provide benefits as the application evolves.
- **Domain purity:** Prevent queries from introducing persistence or projection concerns into the domain layer.
- **Read model evolution:** Evaluate when dedicated projections or separate read stores become justified.
- **Handler consistency:** Keep command and query handlers focused on a single responsibility.

## References

- ADR-001: Adopt a monolith architecture
- ADR-002: Use Clean Architecture
- Greg Young — CQRS Documents
- Martin Fowler — CQRS
- Microsoft — CQRS Pattern