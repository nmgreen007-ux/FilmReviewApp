# Film Review App — Product Requirements Document

**Version:** 1.0  
**Status:** In progress  
**Purpose:** Skills demonstration project

---

## Overview

A single-page web application that displays details for a film and allows users to submit reviews and rankings. The project exists to demonstrate full-stack development skills across a .NET API backend and a React frontend, using SQLite for persistence.

---

## Goals

- Build a complete, working full-stack application within a focused two-day window
- Demonstrate clean architecture, layered separation, and sensible technical decisions
- Produce something presentable on GitHub with minimal scope but genuine depth

---

## Out of Scope

- User authentication and accounts
- Multiple films
- Admin or moderation tooling
- Mobile-optimised design (though it should not break on mobile)

---

## Film Detail Display

The page displays a single, hardcoded film. No browsing, searching, or switching between films is required.

The film detail section shows:

- A poster image
- A plot summary
- Two main cast members

The film data is seeded directly into the database and does not need to be editable.

---

## Reviews

### Submission

Users may submit a review comprising:

- A written note (required)
- A ranking from 1 to 10 (required)
- An optional display name — if left blank, the review is posted as *Anonymous*

Validation is applied at both the API and UI layers. Submitting an incomplete review should return a clear, user-friendly error message.

### Display

Reviews are displayed beneath the film detail section, paginated at 10 per page, ordered newest first.

Each review shows:

- The written note
- The ranking out of 10
- The display name or *Anonymous*
- The date submitted

### Average Ranking

An average ranking is calculated from all submitted reviews and displayed prominently beneath the film details, updating each time a new review is submitted.

---

## AI Review Summary *(stretch goal)*

If time permits, a short AI-generated summary of the review notes is displayed beneath the average ranking. This should reflect the general sentiment of the reviews in two or three sentences.

This feature is explicitly a stretch goal. It should be built last and dropped if it jeopardises the quality of the core application.

---

## Technical Constraints

| Layer | Technology |
|---|---|
| Backend | .NET 10 Web API |
| ORM | Entity Framework Core |
| Database | SQLite |
| Frontend | React (TypeScript) |
| API Docs | Swagger |

The data layer should be structured so that switching from SQLite to SQL Server requires only a connection string change.

---

## What Done Looks Like

- The page loads and displays film details
- A user can submit a review and see it appear in the list
- Pagination works correctly
- The average ranking updates on submission
- The codebase is clean, readable, and structured as it would be in a professional context
- A clear README exists explaining the project and key decisions

---

## Deployment

This project is designed to run locally and does not require a live deployment. The ADRs document how the application would be deployed to Azure in a production context.

If deployed publicly, a review retention policy would be required to periodically cull old reviews and manage database growth. This is out of scope for the current project.
