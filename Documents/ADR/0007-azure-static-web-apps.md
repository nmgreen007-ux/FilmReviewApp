# ADR-0007: Azure Static Web Apps for Frontend Hosting

## Status
Accepted

## Context
The Film Review App has a React frontend that needs to be served to the browser. A decision was needed on how to host and deliver the built React application.

## Decision
The React application is hosted and served via Azure Static Web Apps, separately from the .NET API.

## Reasons
- Separating the frontend from the API reflects a more realistic production architecture and demonstrates awareness of how full-stack applications are structured at scale
- Azure Static Web Apps is purpose-built for serving static files and integrates naturally with the Microsoft stack
- Independent deployment — frontend and API can be released separately without touching each other
- In a production environment, static files served via a CDN are delivered from edge nodes close to the user, improving performance

## Alternatives Considered
For a project of this size, serving the built React files directly from the .NET API via `wwwroot` would have been entirely sufficient. This approach was considered and deliberately set aside in favour of a more realistic architecture that better demonstrates full-stack deployment practices.

## Consequences
- The frontend and API are independently deployable
- The architecture reflects what would be expected in a professional production environment
- A small amount of additional deployment configuration is required compared to serving files from the API
