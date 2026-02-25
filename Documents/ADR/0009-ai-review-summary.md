# ADR-0009: AI Review Summary

## Status
Accepted

## Context
The Film Review App displays user submitted reviews. An opportunity was identified to enhance the experience by providing an AI-generated summary of the review notes, giving users a quick overview of the general sentiment without reading every review individually.

## Decision
Azure OpenAI is used to generate a short summary of the review notes. The integration is handled by the .NET API — the React frontend never communicates with Azure OpenAI directly. The AI summary is treated as a stretch goal and is built last, after the core application is complete.

## Reasons
- Azure OpenAI is the natural choice on the Microsoft stack and keeps all infrastructure within the Azure ecosystem
- Handling the integration server-side keeps the API key secure — it is never exposed to the browser
- A single integration point in the API is simpler to maintain and reason about than a direct frontend-to-LLM call
- Treating it as a stretch goal ensures the quality of the core application is not compromised if time is limited

## Alternatives Considered
No alternative LLM providers were considered. Azure OpenAI is the standard choice on the Microsoft stack and aligns with the existing technology decisions made for this project.

## Consequences
- The API key is kept server-side and is never exposed to the client
- The React frontend consumes the summary via a standard API call, with no knowledge of the underlying LLM integration
- If the AI summary is not completed, the core application remains fully functional and complete
