# ADR-001: Adopt a monolith architecture

## Status

Accepted <!-- Proposed | Accepted | Rejected | Deprecated | Superseded -->

## Date

2026-07-19

## Context

Nutri Metrics is a lightweight calorie-tracking system. Its core functionality consists of:

- Querying food/calorie data from an external provider (CalorieNinjas API).
- Storing daily calorie entries per user.
- Retrieving historical data by date range to feed simple dashboards in a React SPA.

The system does not currently require asynchronous processing, event-driven communication, or messaging infrastructure. There are no notification requirements, no cross-team ownership boundaries, and no independent scaling needs between functional areas at this stage. The consumer side is a single React SPA, not multiple independent client applications requiring separately deployable backend services.

Given this scope, adopting a distributed architecture upfront would introduce coordination and operational overhead disproportionate to the system's actual complexity.

## Decision

We will build Nutri Metrics as a single deployable monolith, internally organized into well-bounded modules (e.g., Identity, CalorieTracking). This preserves a clear separation of concerns within the codebase while avoiding the operational cost of a distributed system. Module boundaries will be designed so that, if scaling or ownership needs change in the future, individual modules could be extracted into independent services with a manageable migration path.

## Alternatives Considered

### Option A: Monolith (selected)

**Pros**

- **Lower operational complexity:** A single deployable unit avoids the need for service orchestration, distributed tracing, or inter-service network management.
- **Faster initial development:** No overhead from designing service contracts, handling network failures between services, or managing distributed transactions.
- **Sufficient for current scope:** The system's functionality (external API lookups, per-user daily records, date-range queries) does not require independent scaling or isolated failure domains.
- **Simpler local development and testing:** Running and debugging a single application is significantly easier than coordinating multiple services.

**Cons**

- **Single deployment unit:** All modules are deployed together, so a change in one module requires redeploying the whole application.
- **Shared runtime resources:** Modules share the same process and database connection pool, which could become a bottleneck if one module's load grows disproportionately.
- **Potential for boundary erosion:** Without discipline, internal module boundaries can degrade over time into a tightly coupled "big ball of mud."

### Option B: Microservices

**Pros**

- **Independent scalability:** Each service could scale according to its own load profile.
- **Independent deployability:** Teams could deploy services independently without coordinating releases.
- **Technology flexibility:** Each service could use a different stack suited to its specific needs.

**Cons**

- **Over-engineering for current scope:** The system has no distinct scaling needs, ownership boundaries, or team separation that would justify the added complexity.
- **Operational overhead:** Requires service discovery, inter-service communication, distributed monitoring, and infrastructure for orchestration (e.g., containers, API gateway).
- **Increased latency and failure surface:** Cross-service calls introduce network latency and additional failure modes not present in an in-process monolith.
- **Higher infrastructure cost:** Running and maintaining multiple deployable services is not justified for a system of this size.

### Option C: Event-Driven Architecture

**Pros**

- **Asynchronous decoupling:** Producers and consumers of events don't need to know about each other directly.
- **Good fit for reactive workflows:** Well suited for systems requiring notifications, real-time updates, or complex workflows triggered by state changes.

**Cons**

- **Not required by current use cases:** Nutri Metrics has no notification, messaging, or asynchronous workflow requirements — all operations (food lookup, entry storage, date-range retrieval) are synchronous by nature.
- **Added infrastructure:** Requires a message broker (e.g., Kafka, RabbitMQ) and associated operational knowledge.
- **Debugging complexity:** Tracing a request across asynchronous event chains is harder than following a direct, synchronous call flow.

## Positive

- **Reduced operational overhead:** No need for service orchestration, message brokers, or distributed infrastructure.
- **Faster time to delivery:** Development effort is focused on business functionality rather than distributed-systems concerns.
- **Simplicity aligned with actual requirements:** The architecture matches the system's real complexity — a synchronous API backing a single React SPA.
- **Preserves future flexibility:** Well-defined internal module boundaries (per ADR-002's Clean Architecture) keep the door open for future extraction into services if requirements change.

## Negative / Trade-offs

- **Coupled deployments:** All modules ship together, which can slow down releases as the codebase grows.
- **Shared failure domain:** An unhandled issue in one module can potentially affect the availability of the whole application.
- **Requires future re-evaluation:** If usage patterns, team structure, or scaling needs change significantly, this decision will need to be revisited.

## Items to Monitor

- **Module coupling:** Ensure internal modules (Identity, CalorieTracking) remain loosely coupled and don't develop hidden cross-module dependencies.
- **Database load:** Monitor whether a single MySQL instance remains sufficient as usage grows, per the scalability concerns noted in ADR-002.
- **Scope growth:** Watch for emerging requirements (notifications, async processing, multiple client types) that could invalidate the assumptions behind this decision.

## References

- ADR-002: Use clean architecture
- Sam Newman — *Building Microservices*
- Martin Fowler — *MonolithFirst*