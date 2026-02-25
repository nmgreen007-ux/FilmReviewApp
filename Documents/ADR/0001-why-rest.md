# ADR-0001: Why REST

## Status
Accepted

## Context
The Film Review App requires an API layer between the React frontend and the SQLite database. A decision was needed on the architectural style for that API.

## Decision
REST was chosen as the API style for this project.

## Reasons
- REST is the industry standard for this type of application and aligns with existing team experience
- The simple request/response nature of the application — retrieving film details, submitting reviews, paginating results — maps naturally to REST's resource-based model
- REST is natively supported by .NET 10 Web API with minimal configuration
- Swagger/OpenAPI documentation tooling integrates directly with REST endpoints, providing clear API documentation out of the box

## Alternatives Considered
Given the simplicity of this project and its purpose as a skills demonstration, no alternative API styles were considered. REST was the natural and only choice.

## Consequences
- API endpoints are predictable and easy to document via Swagger
- The React frontend can consume the API using standard `fetch` calls with no additional libraries
- Switching to a different API style in future would require a rewrite of the API layer, though this is not anticipated
