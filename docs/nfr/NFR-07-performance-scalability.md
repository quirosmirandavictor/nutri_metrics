# NFR-07: Performance & Right-Sizing per Deployment

**Category:** Performance
**Priority:** Should-have

## Description
Because each client runs on its own dedicated Azure resources, scalability is no longer about one shared system absorbing every gym's load at once — it is about each individual deployment staying responsive as *that one client's* usage grows, while not being over-provisioned (and over-billed) for clients with lighter usage.

## Acceptance Criteria
- p95 response time for authenticated CRUD endpoints (`/api/Food/*`) stays under 300 ms under the expected load of a single client's tier (define per tier from NFR-11, e.g., "Starter" ≈ up to 300 concurrent end users).
- `GetByDateRange` and `Search` endpoints support pagination (`page`, `pageSize`) instead of returning the full result set; a sane default and maximum page size is enforced server-side.
- Database queries used by these endpoints are covered by appropriate indexes (e.g., `FoodItems(UserId, CreatedAt)`).
- Each deployment's compute tier supports autoscaling within its own client boundary (e.g., Azure App Service/Container Apps scale rules based on that client's CPU/request metrics), without any cross-client capacity sharing.
- Load/performance tests are run per sizing tier (not per client) before major releases, and results are tracked over time to catch regressions that would affect every deployed client at once.

## Rationale (summary)
`GetFoodItemsByDateRangeQuery` and `SearchFoodQuery` currently return unbounded collections directly from the repository/external API with no pagination parameters — this remains a risk regardless of deployment model, since a single client's data can still grow large over time (months of logged meals for hundreds of end users). What changes with the isolated-deployment model is the scaling unit: instead of designing for aggregate load across all gyms, each environment must be right-sized and independently scalable for its own client's growth curve.
