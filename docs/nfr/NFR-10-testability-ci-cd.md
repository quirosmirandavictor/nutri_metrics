# NFR-10: Testability & Continuous Integration

**Category:** Maintainability
**Priority:** Must-have

## Description
The codebase must be verifiably correct through automated tests, and every change must be validated by a build pipeline before reaching a shared or production environment.

## Acceptance Criteria
- Unit test coverage exists for each module's Application layer (command/query handlers) with a minimum coverage threshold agreed by the team (e.g., 70% on Application/Domain layers).
- Integration tests cover the `Identity` and `CalorieTracking` HTTP endpoints against a real or containerized MySQL instance (e.g., Testcontainers).
- A CI pipeline (GitHub Actions or equivalent) runs build + tests + static analysis on every pull request and blocks merges on failure.
- Contract/regression tests exist for the CalorieNinjas and LibreTranslate integrations using recorded/mocked responses, so external outages don't block the pipeline.
- A staging environment mirrors production configuration (minus secrets) so migrations and new features are validated before customer-facing release.
- The CI/CD pipeline builds a single application artifact/image once, then promotes and deploys that same artifact to each client's isolated Azure environment (via the IaC in NFR-08), so every client runs a known, consistent version rather than being rebuilt per client.

## Rationale (summary)
No test projects were found anywhere in the solution, and no CI/CD configuration (GitHub Actions, Azure Pipelines, etc.) is present. For a Clean Architecture codebase whose explicit goal is long-term maintainability and eventual microservice extraction, the absence of automated tests and a CI gate is the single biggest risk to safely evolving the system as more gyms depend on it.
