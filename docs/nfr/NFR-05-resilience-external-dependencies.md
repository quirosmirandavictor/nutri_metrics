# NFR-05: Resilience Against External Dependency Failures

**Category:** Reliability
**Priority:** Must-have

## Description
Calls to external services (CalorieNinjas nutrition API, LibreTranslate) must not degrade or take down the core API when those services are slow, rate-limited, or unavailable.

## Acceptance Criteria
- All outbound `HttpClient` calls (CalorieNinjas, LibreTranslate) have explicit connect/response timeouts (e.g., via `HttpClient.Timeout` or a resilience pipeline).
- A retry policy with exponential backoff and jitter is applied to transient failures (e.g., via `Microsoft.Extensions.Http.Resilience` / Polly), bounded to avoid amplifying an outage.
- A circuit breaker trips after repeated failures to a dependency and fails fast instead of queuing requests indefinitely.
- If the translation service is unavailable, food search degrades gracefully (e.g., falls back to the original-language query) instead of throwing an unhandled exception.
- Dependency health/latency is exposed as metrics (see NFR-09) so degradation is visible before it causes an outage.

## Rationale (summary)
`CalorieNinjasHttpClient` and the LibreTranslate integration currently have no visible timeout, retry, or circuit-breaker configuration beyond a single `try/catch` around one HTTP call. A slow or failing third-party dependency today would directly translate into slow or failing requests for every gym using the platform at once.
