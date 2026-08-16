# NFR-01: Authentication & Secrets Hardening

**Category:** Security
**Priority:** Must-have (blocking for production release)

## Description
The API's authentication mechanism (JWT issuance/validation, password policy, secret management) must be hardened so that no environment can run with a default, weak, or hardcoded secret, and credentials must resist brute-force and casual guessing attacks.

## Acceptance Criteria
- Application **fails to start** (fail-fast) in any non-development environment if `Jwt:Secret` is missing, shorter than 32 bytes, or matches the known default/sample value.
- JWT signing secret is loaded exclusively from a secure secret store — Azure Key Vault in every client's production environment, User Secrets only in local development — never committed to source control or `appsettings.json`. Each client deployment has its own Key Vault instance and its own unique secret; secrets are never reused across client environments.
- Password policy enforces a minimum of 8 characters **plus** at least one non-alphabetic requirement (digit, symbol, or mixed case), configurable per deployment.
- Account lockout policy (attempts/window) is documented and tested; failed-login attempts are logged without leaking whether the email exists.
- Refresh-token or short-lived access-token strategy is defined so a leaked token has a bounded blast radius (e.g., ≤ 60 minutes).
- Secrets rotation procedure is documented (how to rotate `Jwt:Secret` without invalidating all sessions abruptly).

## Rationale (summary)
The current configuration falls back to a hardcoded sample JWT secret and a permissive password policy when configuration values are absent, and multiple environments read secrets directly from `appsettings.json`. This is a direct impersonation/token-forgery risk once the product is distributed outside a controlled dev environment.
