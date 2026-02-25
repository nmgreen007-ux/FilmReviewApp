# ADR-0005: Testing Approach

## Status
Accepted

## Context
The Film Review App requires a testing strategy that demonstrates awareness of different test types, keeps the codebase reliable, and is achievable within the time constraints of a skills demonstration project.

## Decision
xUnit is used as the testing framework. Unit tests cover service layer logic in isolation. A small number of integration tests use `WebApplicationFactory` to test API endpoints end-to-end in memory.

## Reasons
- xUnit is the standard testing framework in the .NET ecosystem and is well supported by the .NET tooling
- Unit testing the service layer in isolation is straightforward given the use of DI and interfaces throughout — dependencies can be mocked cleanly at layer boundaries
- `WebApplicationFactory` is the standard .NET approach for integration testing — it spins up the full application pipeline in memory, allowing real HTTP calls to be made against the API without a running server
- Together, the two test types demonstrate awareness of the testing pyramid without over-investing in test coverage for a demo project

## Alternatives Considered
Given the simplicity of this project and its purpose as a skills demonstration, no alternative testing frameworks were considered. xUnit and `WebApplicationFactory` are the natural and only choices for a modern .NET project.

## Consequences
- Service logic can be verified in isolation via unit tests
- At least one integration test demonstrates the full request/response cycle through the API
- The testing structure is consistent with what would be expected in a professional .NET codebase
