# NFR-03: Rate Limiting & Abuse Protection

**Category:** Security / Cost Management
**Priority:** Must-have

## Description
Even within a single, isolated client deployment, the API must throttle requests to protect authentication endpoints from brute-force/credential-stuffing attacks and to protect that client's own metered external API usage (CalorieNinjas) and Azure spend from being exhausted by abusive, compromised, or malfunctioning traffic.

## Acceptance Criteria
- A rate-limiting middleware (e.g., ASP.NET Core `Microsoft.AspNetCore.RateLimiting`) is applied globally within each deployment, with stricter limits on `POST /api/Auth/login` and `POST /api/Auth/register` (e.g., N attempts per minute per IP/email).
- Exceeding the limit returns HTTP 429 with a `Retry-After` header, not a generic 500 or a silent hang.
- A per-deployment daily/hourly quota exists for `/api/Food/search`, so a single compromised account or buggy client integration cannot exhaust that gym's CalorieNinjas allowance or trigger unexpected Azure outbound/API cost.
- Rate-limit thresholds are set as IaC parameters (see NFR-08) so they can differ by client tier (e.g., a larger gym gets a higher ceiling) without a code change.
- Rate-limit violations are logged and visible in the fleet observability stack (see NFR-09), tagged with the client that triggered them.

## Rationale (summary)
No rate-limiting middleware exists today, and `AuthController.Login` relies solely on the ASP.NET Identity lockout counter with no IP-level throttling. Even though each gym now runs in isolation, that isolation does not remove the risk — an abusive actor, a compromised account, or a bug in one client's own integration could still exhaust its CalorieNinjas quota or drive up that specific deployment's Azure costs. The isolation only ensures the impact stays contained to that one client instead of spreading to others.
