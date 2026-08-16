# NFR-02: Transport Layer Security (HTTPS Enforcement)

**Category:** Security
**Priority:** Must-have (blocking for production release)

## Description
All traffic to and from the API — including JWTs, credentials, and personal nutrition data — must be encrypted in transit in every environment reachable from outside a trusted private network.

## Acceptance Criteria
- `Http:UseHttpsRedirection` (or equivalent) is hardcoded to `true`/enforced in Staging and Production configuration and cannot be silently disabled by a missing config key.
- HSTS (`Strict-Transport-Security`) is enabled for all non-development environments.
- TLS certificates are managed and renewed automatically via Azure-managed certificates (App Service/Front Door/Application Gateway), with alerting before expiration, consistently across every client deployment provisioned by the IaC templates (NFR-08).
- Any client (mobile app, web app, third-party integration) connecting over plain HTTP is rejected or redirected, not silently accepted.
- Internal service-to-service calls (e.g., to the translation service) use TLS when crossing a network boundary that is not fully trusted/private.

## Rationale (summary)
`UseHttpsRedirection` is currently optional and defaults to `false`, and the sample connection strings/API keys are read in plaintext from configuration. For a product handling authentication credentials and client health/nutrition data, encryption in transit cannot be an opt-in setting.
