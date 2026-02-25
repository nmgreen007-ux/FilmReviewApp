# ADR-0008: React as a Single-Page Application

## Status
Accepted

## Context
The Film Review App requires a frontend. A decision was needed on both the architectural pattern for the frontend and the technology used to build it.

## Decision
React was chosen as the frontend technology, built as a Single-Page Application (SPA) using TypeScript.

## Reasons
- React is widely adopted across the industry and produces a recognisable, transferable frontend for a skills demonstration project
- A SPA provides a dynamic, reactive user experience without full page reloads — appropriate for a UI where reviews are submitted and the page updates in response
- React's component-based model maps naturally to the UI structure of this application — film detail, review form, review list, and average ranking are all discrete, reusable components
- TypeScript is standard practice when building React on the Microsoft stack, providing type safety and improved tooling

## Alternatives Considered

**ASP.NET MVC** — ruled out early. MVC would require a full page reload on each interaction, which is not appropriate for the dynamic, reactive user experience this application requires.

**Blazor** — a natural fit on the Microsoft stack and would have been the simpler choice. Set aside in favour of React, which is more widely adopted across the industry and better demonstrates broad frontend capability for a public portfolio project.

## Consequences
- The frontend is a modern, component-based SPA that will be recognisable to any frontend developer reviewing the repository
- The separation between frontend and API is clean — the React app communicates with the .NET API via HTTP, with no server-side rendering concerns
- The project demonstrates full-stack capability across both the Microsoft backend stack and modern frontend development
