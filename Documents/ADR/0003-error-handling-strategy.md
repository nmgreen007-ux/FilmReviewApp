# ADR-0003: Error Handling Strategy

## Status
Accepted

## Context
The Film Review App requires a consistent approach to handling and communicating errors across the API and the React frontend. Decisions were needed on how validation failures, not-found responses, and unexpected exceptions are handled and surfaced to the user.

## Decision
ProblemDetails is used as the standard error response format across all API error responses, following RFC 7807. Global exception handling middleware is used to catch unhandled exceptions, log them, and return a safe ProblemDetails response to the client.

## Reasons
- ProblemDetails is the modern .NET standard for structured API error responses and is built into .NET 10
- A consistent error shape means the React frontend always knows what to expect and can handle errors predictably
- Global middleware ensures no unhandled exception reaches the client with internal details exposed
- Logging exceptions at the middleware level provides visibility without additional effort per endpoint

## Alternatives Considered
Given the simplicity of this project and its purpose as a skills demonstration, no alternative error handling approaches were considered. ProblemDetails is the natural and only choice for a modern .NET API.

## Consequences
- Error responses are consistent and predictable across the entire API
- Internal implementation details are never exposed to the client
- Unexpected exceptions are logged for visibility
- The frontend has a reliable contract for handling and displaying errors
