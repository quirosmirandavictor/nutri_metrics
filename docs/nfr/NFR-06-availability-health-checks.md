# NFR-06: Availability & Health Monitoring

**Category:** Reliability / Operability
**Priority:** Should-have

## Description
The API must expose machine-checkable health status so orchestration platforms (Docker/Kubernetes/load balancers) and on-call staff can detect and react to unhealthy instances automatically.

## Acceptance Criteria
- A `/health/live` endpoint reports process liveness (no dependency checks).
- A `/health/ready` endpoint reports readiness, including MySQL connectivity for both DbContexts and reachability of critical external dependencies.
- Target uptime SLA is defined (e.g., 99.5% monthly) appropriate for a B2B tool gyms rely on during business hours.
- Each client's Azure compute service (App Service/Container Apps health probes, or Application Gateway/Front Door health checks) is wired to these endpoints, independently per deployment.
- Planned maintenance windows are communicated in advance to gym/professional administrators.

## Rationale (summary)
No health-check endpoints or explicit uptime target currently exist in the codebase. As soon as the API is deployed behind a load balancer, in containers, or with auto-scaling, the platform needs a reliable, low-cost way to know an instance is actually able to serve traffic (including DB and dependency connectivity), not just that the process is running.
