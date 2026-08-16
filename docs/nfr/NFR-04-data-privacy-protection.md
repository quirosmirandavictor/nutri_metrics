# NFR-04: Personal & Nutrition Data Privacy Protection

**Category:** Privacy / Compliance
**Priority:** Must-have

## Description
Client nutrition, dietary, and body-metric data handled on behalf of a gym or nutrition professional is personal (and in some jurisdictions health-adjacent) data. Even though the per-client Azure deployment model already provides strong physical/logical data separation between clients, each individual deployment must still protect that client's data against unauthorized access, exposure in logs, and unbounded retention.

## Acceptance Criteria
- No personal data (email, food logs, biometric data) is written to logs or traces in plaintext; PII fields are redacted/masked before reaching the logging or tracing pipeline — including the centralized fleet observability destination (NFR-09), which aggregates data from every client.
- A documented data-retention and deletion policy exists per client (e.g., a gym's client data is deletable on request, "right to be forgotten"), executable without affecting any other client's environment.
- Error responses returned to API clients never leak internal exception details (stack traces, SQL fragments, raw `ex.Message`) that could reveal data structure or infrastructure details.
- Each client's database credentials and connection details are stored exclusively in that client's own Azure Key Vault instance, never shared across deployments.
- A privacy notice / data-processing agreement template exists for gyms/professionals to give to their end clients, since NutriMetrics acts as a data processor on their behalf, and the isolated-deployment model should be referenced as a positive data-residency/segregation guarantee.

## Rationale (summary)
`AuthService` currently returns raw exception messages (`ex.Message`) in API responses, which risks leaking internal details regardless of deployment model. The isolated-per-client architecture removes the *cross-tenant* data leakage risk almost entirely (there is no shared database or shared application instance to leak across), but it does not remove the need for baseline data protection *within* each deployment — especially once telemetry from every client is centralized for fleet observability (NFR-09), which reintroduces a single place where PII from multiple clients could be exposed if not properly redacted.
