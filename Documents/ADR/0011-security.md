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

## Phase 2 — Implementation

### Identity Provider
**Microsoft Entra External ID (B2C)** was used as the identity provider. Regular Entra ID (Azure AD) is designed for organisational users with work or school accounts. B2C is designed for consumer-facing applications where users are members of the public, supporting email/password registration and social login providers. For a public film review application, B2C is the correct choice.

### API
Read endpoints remain open — no authentication is required to retrieve film details or reviews:

- `GET /films/{id}` — open
- `GET /reviews` — open

The write endpoint requires a valid bearer token:

- `POST /reviews` — `[Authorize]`

The `Microsoft.Identity.Web` middleware was configured in `Program.cs` to validate bearer tokens issued by the B2C tenant. No changes to the service or data layers were required — authentication is enforced entirely at the API boundary.

### React SPA
**Microsoft Authentication Library (MSAL)** was integrated into the React SPA to handle the authentication flow. MSAL was introduced at the application root via `MsalProvider`. The review form is hidden and replaced with a sign-in prompt until the user is authenticated. On submission, MSAL attaches the bearer token to the API request via the `Authorization: Bearer` header.

Token acquisition uses a silent-first strategy — MSAL attempts `acquireTokenSilent` and falls back to `acquireTokenPopup` if the silent attempt fails. Redirect-based login and logout are used for the main authentication flow.

### Swagger UI
Swagger was configured with the B2C OAuth2 flow, adding an Authorize button to the Swagger UI. Once authenticated, Swagger attaches the bearer token to all requests automatically. The B2C redirect URI (`https://localhost:{port}/swagger/oauth2-redirect.html`) is registered as an allowed redirect URI in the B2C app registration.

### Azure Configuration
Phase 2 required the following Azure configuration, which has been completed:

- A B2C tenant (`ngExternalUsers.onmicrosoft.com`) with a sign-up/sign-in user flow configured
- Two app registrations — one for the API (`b7e984e6-aca0-4e82-af92-85f19a2606de`), one for the SPA (`c8c9c88f-eba2-4098-b20e-a6cb6477010e`)
- The SPA app registration granted permission to call the API via the `reviews.write` scope
- Allowed redirect URIs configured for both the SPA and Swagger UI

## Consequences
- Phase 1 accepted the risk of unauthenticated write access in a local development context — this gap is now closed
- Read endpoints remain open in both phases — no authentication friction for casual visitors
- Authentication was introduced without modifying the service or data layers, confirming the architecture's layered separation held as intended
- Phase 2 is complete: `POST /api/films/{filmId}/reviews` enforces `[Authorize]`; all GET endpoints remain open
