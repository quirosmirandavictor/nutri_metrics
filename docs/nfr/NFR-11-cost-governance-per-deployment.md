# NFR-11: Cost Governance & Resource Efficiency per Client Deployment

**Category:** Operability / Cost Management
**Priority:** Should-have

## Description
Since infrastructure cost is now incurred per client rather than shared across a multi-tenant system, the cost of each deployment must be predictable, right-sized to that client's actual usage (a small gym vs. a multi-location chain), and visible for billing/margin decisions.

## Acceptance Criteria
- At least two Azure sizing tiers are defined (e.g., "Starter" for a single-location gym, "Growth" for a chain/high-volume professional), each with documented compute/DB SKUs and expected monthly cost.
- Compute resources scale down (or to zero, where the Azure service supports it, e.g., Container Apps scale-to-zero) during low-traffic periods for smaller clients, to avoid paying for idle capacity 24/7.
- Azure cost is tagged and reportable per client (via resource group/tags), so cost-per-client can be reconciled against what that client is billed.
- A budget alert is configured per client resource group to catch runaway costs (e.g., a misbehaving integration looping calls to CalorieNinjas) before it becomes a large bill.
- The IaC templates from NFR-08 expose sizing as parameters, so upgrading a client from one tier to another does not require re-architecting their deployment.

## Rationale (summary)
An isolated-per-client architecture trades the cost-sharing efficiency of multi-tenancy for operational isolation — which is the right trade-off for data separation and blast-radius containment, but it means infrastructure spend scales with the number of clients. Without explicit sizing tiers and cost visibility per deployment, it becomes difficult to know whether a given client is profitable or whether a specific integration (e.g., unbounded CalorieNinjas calls) is silently inflating one client's Azure bill.
