# ADR-0004: Logging & Observability

## Status
Accepted

## Context
The Film Review App requires a consistent approach to logging application events, errors, and exceptions. A decision was needed on how logging is implemented and where log output is directed.

## Decision
The built-in .NET `ILogger` abstraction is used throughout the application for all logging. Log output is directed to the console for this project.

## Reasons
- `ILogger` is built into .NET 10 and requires no additional dependencies
- Using the `ILogger` abstraction rather than a concrete logging implementation means the underlying log destination can be changed without modifying application code
- Console logging is sufficient for a local development and demonstration context
- In a production environment, the application would be connected to Azure Application Insights by adding the Application Insights SDK — `ILogger` integrates automatically, shipping all log output without requiring any changes to logging statements in the codebase

## Alternatives Considered
Given the simplicity of this project and its purpose as a skills demonstration, no alternative logging libraries were considered. The built-in `ILogger` abstraction is the natural and only choice.

## Consequences
- All application layers log consistently via the same abstraction
- Exceptions caught by the global error handling middleware are logged before returning a safe response to the client
- The path to production-grade observability via Azure Application Insights is a NuGet package and a connection string — no code changes required
