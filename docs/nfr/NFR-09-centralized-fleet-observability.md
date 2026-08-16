# NFR-09: Centralized Fleet Observability & Alerting

**Category:** Operability
**Priority:** Should-have

## Description
With every gym/professional running an independent deployment, the team needs a single pane of glass to know the health of the entire fleet — instead of having to check N separate Azure resources individually to find out which client is degraded or down.

## Acceptance Criteria
- All per-client deployments export telemetry (via the existing OpenTelemetry pipeline) to a centralized destination (e.g., a shared Azure Monitor / Application Insights workspace or Log Analytics workspace), tagged with a client identifier.
- A single dashboard shows, per client: uptime, error rate, p95 latency, and external-dependency health, filterable by client.
- Alerting rules fire per client but route to the operations team, not to the client — the client-facing story stays "your platform is monitored," not "you must set up your own monitoring."
- Correlation/request IDs remain traceable within a client's own deployment (no cross-client correlation needed, since environments are isolated), but the client tag lets the team pivot from "fleet view" to "this specific client's traces."
- A recurring fleet health report (e.g., weekly) summarizes SLA compliance across all client deployments.

## Rationale (summary)
The project already instruments tracing, metrics, and logging via OpenTelemetry — a strength to build on. The isolated-per-client model changes the challenge from "understanding one shared system" to "understanding N independent copies of the same system without opening N separate Azure blades." Centralizing telemetry with a client tag is what makes an isolated-deployment strategy operable at scale as the number of gyms grows.
