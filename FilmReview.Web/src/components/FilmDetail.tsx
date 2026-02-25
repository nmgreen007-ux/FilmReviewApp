import { useState, useEffect } from "react";
import type { FilmDetailDto, ReviewsListDto } from "../types/index";
import Overview from "./Overview";
import ReviewForm from "./ReviewForm";
import ReviewList from "./ReviewList";
import { getFilmDetails, getReviews } from "../api/client";

interface FilmDetailProps {
  filmId?: number;
}

function FilmDetail({ filmId = 1 }: FilmDetailProps) {
  const [film, setFilm] = useState<FilmDetailDto | null>(null);
  const [reviews, setReviews] = useState<ReviewsListDto | null>(null);
  const [currentPage, setCurrentPage] = useState(0);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Load film details and reviews when component mounts or when filmId/currentPage changes
  useEffect(() => {
    const loadData = async () => {
      setIsLoading(true);
      setError(null);

      try {
        const filmData = await getFilmDetails(filmId);
        setFilm(filmData);

        const reviewsData = await getReviews(filmId, currentPage);
        setReviews(reviewsData);
      } catch (err) {
        setError(
          err instanceof Error ? err.message : "Failed to load film details",
        );
      } finally {
        setIsLoading(false);
      }
    };

    loadData();
  }, [filmId, currentPage]);

  //change current page when user clicks pagination buttons
  const handlePageChange = (page: number) => {
    setCurrentPage(page);
  };

  //refresh the reviews and film details after a new review is submitted
  const handleReviewSubmitted = async () => {
    try {
      const reviewsData = await getReviews(filmId, 0);
      setReviews(reviewsData);
      setCurrentPage(0);

      const filmData = await getFilmDetails(filmId);
      setFilm(filmData);
    } catch (err) {
      setError("Failed to refresh data after review submission");
    }
  };

  if (isLoading) {
    return (
      <div className="container mt-5">
        <p>Loading...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div className="container mt-5">
        <div className="alert alert-danger">{error}</div>
      </div>
    );
  }

  if (!film || !reviews) {
    return (
      <div className="container mt-5">
        <p>No film data available</p>
      </div>
    );
  }

  // Render film details, review form, and review list
  return (
    <div className="film-detail">
      <Overview film={film} />
      <ReviewForm filmId={filmId} onSubmitSuccess={handleReviewSubmitted} />
      <ReviewList
        reviews={reviews.reviews}
        currentPage={currentPage}
        totalPages={reviews.totalPages}
        onPageChange={handlePageChange}
      />
    </div>
  );
}

export default FilmDetail;
