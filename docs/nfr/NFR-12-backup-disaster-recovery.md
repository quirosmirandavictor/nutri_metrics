# NFR-12: Backup & Disaster Recovery

**Category:** Reliability / Data Durability
**Priority:** Must-have

## Description
Loss of the MySQL database (identity + calorie-tracking data) would mean every partnered gym permanently loses its clients' historical nutrition records. The system must define and test a recovery process with an explicit, agreed data-loss and downtime tolerance.

## Acceptance Criteria
- Recovery Point Objective (RPO) and Recovery Time Objective (RTO) are defined and agreed with stakeholders (e.g., RPO ≤ 24h, RTO ≤ 4h for the initial commercial tier).
- Automated, encrypted database backups run on a defined schedule per client (e.g., Azure Database for MySQL Flexible Server automated backups, geo-redundant for higher tiers) and are stored in a location independent of that client's primary database instance.
- A restore procedure is documented and tested at least quarterly (a backup that has never been restored is not a verified backup).
- EF Core migrations are reversible or accompanied by a rollback plan for destructive schema changes.
- Gym/professional administrators are informed (in the service agreement) of the backup frequency and what data-loss window they should expect in a worst-case incident.

## Rationale (summary)
No backup, restore, or migration-rollback strategy is documented in the repository, and the database is auto-migrated on every application startup (`InitializeDatabaseAsync`) with no visible safeguard against a destructive migration running directly against production data. For a system that will hold the exclusive record of many gyms' client nutrition histories, this is a business continuity requirement, not just a technical one.
