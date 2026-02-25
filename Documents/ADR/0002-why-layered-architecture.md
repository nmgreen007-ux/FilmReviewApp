# ADR-0002: Why Layered Architecture

## Status
Accepted

## Context
The Film Review App requires a clear structure for organising code across the API backend. A decision was needed on how to separate concerns and structure the application.

## Decision
A layered architecture was chosen, separating the application into three distinct layers:

- **Controllers** — handle HTTP requests and responses
- **Services** — contain business logic
- **Data Access** — handle all database interaction via repositories

Each layer only communicates with the layer directly below it.

## Reasons
- Demonstrates SOLID principles clearly, particularly single responsibility and dependency inversion
- Dependency Injection is used throughout, with interfaces defined in the service layer and implementations registered in `Program.cs`
- Each layer can be reasoned about, tested, and changed independently
- DI makes the codebase straightforward to unit test — services can be tested in isolation by mocking their dependencies

## Alternatives Considered
Given the simplicity of this project and its purpose as a skills demonstration, no alternative architectural patterns were considered. Layered architecture was the natural and only choice.

## Consequences
- The codebase has a clear, predictable structure that is easy to navigate
- Unit testing is straightforward — dependencies can be mocked at any layer boundary
- Adding new features follows a consistent pattern throughout the project
