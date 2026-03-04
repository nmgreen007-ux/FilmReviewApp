# ADR-0011: Security and Authentication

## Status
Accepted

## Context
The Film Review App exposes a `POST /reviews` endpoint that allows any client to submit a review. With no authentication in place, there is no mechanism to identify or restrict who can write to the API. This is a known gap.

Read endpoints are intentionally public — users should be able to browse the film and read reviews without any friction or account requirement. Only review submission requires authentication, which matches the standard pattern for consumer-facing review applications.

## Decision
No authentication or security controls are implemented in Phase 1. The application runs in a local development environment only and is not deployed to a public endpoint. The security shortfall is acknowledged and logged here. Authentication is deferred to Phase 2.

## Reasons
- User authentication is explicitly listed as out of scope in the PRD for this phase
- The application is not deployed publicly — it runs locally for demonstration purposes only
- Implementing authentication correctly takes meaningful time and would risk the quality or completeness of the core application
- Documenting the gap and the path forward is more honest and more useful than applying partial controls that do not meaningfully address the underlying problem

## Phase 2 — Recommended Approach

### Identity Provider
**Microsoft Entra External ID (B2C)** is used as the identity provider. Regular Entra ID (Azure AD) is designed for organisational users with work or school accounts. B2C is designed for consumer-facing applications where users are members of the public, supporting email/password registration and social login providers. For a public film review application, B2C is the correct choice.

### API
Read endpoints remain open — no authentication is required to retrieve film details or reviews:

- `GET /films/{id}` — open
- `GET /reviews` — open

The write endpoint requires a valid bearer token:

- `POST /reviews` — `[Authorize]`

The `Microsoft.Identity.Web` middleware is configured in `Program.cs` to validate bearer tokens issued by the B2C tenant. No changes to the service or data layers are required — authentication is enforced entirely at the API boundary.

### React SPA
**Microsoft Authentication Library (MSAL)** is used in the React SPA to handle the authentication flow. The SPA UI does not change structurally — MSAL is introduced at the application root via `MsalProvider`. The review form is hidden or replaced with a sign-in prompt until the user is authenticated. On submission, MSAL attaches the bearer token to the API request via the `Authorization: Bearer` header.

MSAL supports both redirect and popup login flows — redirect is preferred for SPAs as it avoids browser popup blocking and degrades more gracefully on mobile. In consumer B2C flows there is no silent token acquisition (users are not pre-authenticated via a corporate session), so the redirect flow will always be visible on first login.

### Swagger UI
Swagger is configured with the B2C OAuth2 flow, adding an Authorize button to the Swagger UI. Once authenticated, Swagger attaches the bearer token to all requests automatically. The B2C redirect URI (`https://localhost:{port}/swagger/oauth2-redirect.html`) must be registered as an allowed redirect URI in the B2C app registration.

### Azure Configuration
Phase 2 requires the following Azure configuration:

- A B2C tenant with a sign-up/sign-in user flow configured
- Two app registrations — one for the API, one for the SPA
- The SPA app registration granted permission to call the API app registration
- Allowed redirect URIs configured for both the SPA and Swagger UI

## Consequences
- In Phase 1, any client can submit a review to the API — this is a known and accepted risk for a local development context
- Read endpoints remain open in both phases — no authentication friction for casual visitors
- The architecture is structured so that B2C authentication can be introduced without modifying the service or data layers
- Phase 2 implementation requires: MSAL configuration in the React SPA, B2C tenant and app registration setup, and `Microsoft.Identity.Web` middleware in the .NET API
