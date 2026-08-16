# NFR-08: Infrastructure Isolation & Repeatable Per-Client Provisioning (IaC)

**Category:** Architecture / Operability
**Priority:** Must-have

## Description
Each gym or nutrition professional runs on its own dedicated set of Azure resources (App Service/Container App, database, Key Vault, etc.) rather than sharing an application instance with other clients. Standing up, updating, and decommissioning a client's environment must be fully automated and reproducible — not a manual, one-off process.

## Acceptance Criteria
- The entire per-client stack (compute, database, Key Vault, networking, monitoring resources) is defined as Infrastructure as Code (Bicep or Terraform), parameterized by client name/ID — no resource is created by hand in the Azure Portal for a production client.
- A new client environment can be provisioned end-to-end (infra + first deployment + DB migration) via a single pipeline run, with a defined target time (e.g., ≤ 30 minutes).
- Each client's resources are grouped in their own Azure Resource Group (or subscription, for larger/regulated clients), with naming and tagging conventions that make ownership unambiguous (client ID, environment, cost center).
- Network isolation is enforced per client: each client's database is not reachable from any other client's compute, even though all run the same application image.
- A documented, tested decommissioning procedure exists to safely tear down a client's resources (e.g., on contract termination) without affecting others.
- Application version upgrades are rolled out to client environments through the same pipeline (e.g., ring-based or batched rollout), not by manually redeploying to each one.

## Rationale (summary)
Because the product is deployed as an independent stack per gym/professional rather than as a shared multi-tenant system, the primary architectural risk shifts from "data leaking between tenants in one system" to "operational inconsistency and manual error across many independent systems." Without IaC-driven, repeatable provisioning, onboarding costs grow linearly (or worse) with each new client, and configuration drift between environments becomes a source of hard-to-diagnose bugs and security gaps.
