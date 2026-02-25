import type {
  FilmDetailDto,
  ReviewsListDto,
  CreateReviewDto,
} from "../types/index";

const API_BASE_URL = "/api";

export async function getFilmDetails(filmId: number): Promise<FilmDetailDto> {
  const response = await fetch(`${API_BASE_URL}/films/${filmId}`);
  if (!response.ok) {
    throw new Error(`Failed to fetch film details: ${response.statusText}`);
  }
  return response.json();
}

export async function getReviews(
  filmId: number,
  page: number = 0,
): Promise<ReviewsListDto> {
  const response = await fetch(
    `${API_BASE_URL}/films/${filmId}/reviews?page=${page}`,
  );
  if (!response.ok) {
    throw new Error(`Failed to fetch reviews: ${response.statusText}`);
  }
  return response.json();
}

export async function submitReview(
  filmId: number,
  review: CreateReviewDto,
): Promise<void> {
  const response = await fetch(`${API_BASE_URL}/films/${filmId}/reviews`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(review),
  });

  if (!response.ok) {
    const error = await response.json();
    throw new Error(error.detail || "Failed to submit review");
  }
}
