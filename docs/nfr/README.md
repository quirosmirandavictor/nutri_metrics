# NutriMetrics API — Non-Functional Requirements (NFRs)

This set of documents proposes Non-Functional Requirements for the NutriMetrics backend API, based on a review of its current architecture (modular monolith, Clean Architecture, CQRS + MediatR, JWT auth, MySQL, CalorieNinjas + LibreTranslate integrations) and its intended deployment model on Azure: **each gym or nutrition professional gets its own fully isolated set of Azure resources — a dedicated, independent deployment per client, not a shared multi-tenant system.**

| ID | Title | Category | Priority |
|----|-------|----------|----------|
| NFR-01 | Authentication & Secrets Hardening | Security | Must-have |
| NFR-02 | Transport Layer Security (HTTPS Enforcement) | Security | Must-have |
| NFR-03 | Rate Limiting & Abuse Protection | Security / Cost Management | Must-have |
| NFR-04 | Personal & Nutrition Data Privacy Protection | Privacy / Compliance | Must-have |
| NFR-05 | Resilience Against External Dependency Failures | Reliability | Must-have |
| NFR-06 | Availability & Health Monitoring | Reliability / Operability | Should-have |
| NFR-07 | Performance & Right-Sizing per Deployment | Performance | Should-have |
| NFR-08 | Infrastructure Isolation & Repeatable Per-Client Provisioning (IaC) | Architecture / Operability | Must-have |
| NFR-09 | Centralized Fleet Observability & Alerting | Operability | Should-have |
| NFR-10 | Testability & Continuous Integration | Maintainability | Must-have |
| NFR-11 | Cost Governance & Resource Efficiency per Client Deployment | Operability / Cost Management | Should-have |
| NFR-12 | Backup & Disaster Recovery | Reliability / Data Durability | Must-have |