# ADR-0006: SQLite as the Database

## Status
Accepted

## Context
The Film Review App requires a database for persisting film data and reviews. A decision was needed on which database technology to use.

## Decision
SQLite was chosen as the database for this project, accessed via Entity Framework Core.

## Reasons
- SQLite requires no infrastructure setup — the database is a single file alongside the project
- There is no cost associated with running SQLite, which is appropriate for a skills demonstration project
- EF Core migrations work identically against SQLite and SQL Server — switching to SQL Server in a production environment requires only a connection string change
- The simplicity of the setup keeps the focus on the application code rather than infrastructure

## Alternatives Considered
Given the simplicity of this project and its purpose as a skills demonstration, no alternative databases were considered. SQLite is the natural and only choice for a local development and demo context.

## Consequences
- The database requires no installation or configuration to run the project locally
- The data layer is structured so that switching to SQL Server requires only a connection string change
- SQLite is not appropriate for a production environment at scale, but this is not a concern for this project
