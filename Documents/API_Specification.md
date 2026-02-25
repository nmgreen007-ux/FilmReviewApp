# Film Review App — API Specification

**Version:** 1.0  
**Status:** Proposed  
**Base URL:** `/api`

---

## Overview

This document defines the HTTP contract for the Film Review App API. It covers all endpoints the React SPA requires, including expected request shapes, response shapes, and error behaviour.

Swagger/OpenAPI documentation is generated automatically from the running API and should be treated as the living contract post-implementation. This document exists to define intent before implementation begins.

---

## Conventions

- All request and response bodies are `application/json`
- All error responses follow RFC 7807 ProblemDetails format
- Dates are returned as ISO 8601 strings
- Nullable fields are included in responses with a `null` value rather than omitted
- Pagination is zero-based (page index starts at 0)

---

## Endpoints

### Films

#### `GET /api/films/{filmId}` — `GetFilm`

Returns the details of a single film including cast, average ranking, and AI summary.

**Path Parameters**

| Parameter | Type | Required | Description |
|---|---|---|---|
| `filmId` | `int` | Yes | The ID of the film |

**Response — 200 OK**

```json
{
  "filmId": 1,
  "title": "The Great Adventure",
  "posterUrl": "https://example.com/poster.jpg",
  "plotSummary": "A thrilling journey through unexplored territories...",
  "cast": [
    { "actorId": 1, "name": "Emma Thompson" },
    { "actorId": 2, "name": "Michael Chen" }
  ],
  "averageRanking": 8.7,
  "aiSummary": "Reviewers have praised the film for its stunning visuals and compelling performances.",
  "reviewCount": 3
}
```

`averageRanking` and `aiSummary` are `null` until the first review is submitted.

**Error Responses**

| Status | Reason |
|---|---|
| `404 Not Found` | No film exists with the given ID |

---

### Reviews

#### `GET /api/films/{filmId}/reviews` — `GetReviews`

Returns a paginated list of reviews for a film, ordered newest first.

**Path Parameters**

| Parameter | Type | Required | Description |
|---|---|---|---|
| `filmId` | `int` | Yes | The ID of the film |

**Query Parameters**

| Parameter | Type | Required | Default | Description |
|---|---|---|---|---|
| `page` | `int` | No | `0` | Zero-based page index |

**Response — 200 OK**

```json
{
  "reviews": [
    {
      "reviewId": 3,
      "note": "This film exceeded all my expectations.",
      "ranking": 10,
      "displayName": "Mike Rodriguez",
      "submittedAt": "2026-02-17T09:45:00Z"
    },
    {
      "reviewId": 2,
      "note": "Good movie overall, but the pacing felt a bit slow in the middle.",
      "ranking": 7,
      "displayName": null,
      "submittedAt": "2026-02-16T10:15:00Z"
    }
  ],
  "totalCount": 3,
  "page": 0,
  "totalPages": 1
}
```

`displayName` is `null` when the reviewer did not provide a name. The frontend renders `null` as *Anonymous*.

**Error Responses**

| Status | Reason |
|---|---|
| `404 Not Found` | No film exists with the given ID |

---

#### `POST /api/films/{filmId}/reviews` — `SubmitReview`

Submits a new review for a film. Triggers recalculation of the average ranking and AI summary.

**Path Parameters**

| Parameter | Type | Required | Description |
|---|---|---|---|
| `filmId` | `int` | Yes | The ID of the film |

**Request Body**

```json
{
  "note": "An absolutely thrilling cinematic experience!",
  "ranking": 9,
  "displayName": "Sarah Johnson"
}
```

| Field | Type | Required | Validation |
|---|---|---|---|
| `note` | `string` | Yes | Non-empty |
| `ranking` | `int` | Yes | Between 1 and 10 inclusive |
| `displayName` | `string` | No | If provided, non-empty |

**Response — 201 Created**

No response body. The `Location` header points to `GET /api/films/{filmId}/reviews`.

> The client discards the POST response and immediately calls `GetReviews` to refresh the list. Since this application has no edit, delete, or individual review view, the `reviewId` is never needed by the frontend. Returning the created resource would produce a payload that is always thrown away, so 201 with no body is the honest contract.

**Error Responses**

| Status | Reason |
|---|---|
| `400 Bad Request` | Validation failure — ProblemDetails body with field-level errors |
| `404 Not Found` | No film exists with the given ID |

---

## Error Response Shape

All errors return a ProblemDetails body per RFC 7807.

**Validation failure example (400)**

```json
{
  "type": "https://tools.ietf.org/html/rfc7807",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "ranking": ["Ranking must be between 1 and 10."],
    "note": ["Note is required."]
  }
}
```

**Not found example (404)**

```json
{
  "type": "https://tools.ietf.org/html/rfc7807",
  "title": "Not Found",
  "status": 404,
  "detail": "Film with ID 99 was not found."
}
```

**Unexpected error (500)**

```json
{
  "type": "https://tools.ietf.org/html/rfc7807",
  "title": "An unexpected error occurred.",
  "status": 500
}
```

Internal details are never included in 500 responses.
