import type { ReviewDto } from "../types/index";
import ReviewCard from "./ReviewCard";

interface ReviewListProps {
  reviews: ReviewDto[];
  currentPage: number;
  totalPages: number;
  onPageChange: (page: number) => void;
}

function ReviewList({
  reviews,
  currentPage,
  totalPages,
  onPageChange,
}: ReviewListProps) {
  return (
    <div className="review-list-container">
      <h3>Reviews ({reviews.length})</h3>

      {/* Show message if there are no reviews, otherwise show the list of reviews */}
      {reviews.length === 0 ? (
        <p className="no-reviews">No reviews yet. Be the first to review!</p>
      ) : (
        <div className="reviews">
          {reviews.map((review) => (
            <ReviewCard key={review.reviewId} review={review} />
          ))}
        </div>
      )}

      {/* Show pagination controls only if there are more than 1 page of reviews */}
      {totalPages > 1 && (
        <nav className="pagination-container">
          <ul className="pagination">
            {/* Disable "Previous" button on first page to prevent invalid page changes */}
            <li className={`page-item ${currentPage === 0 ? "disabled" : ""}`}>
              <button
                className="page-link"
                onClick={() => onPageChange(currentPage - 1)}
                disabled={currentPage === 0}
              >
                Previous
              </button>
            </li>

            {/* Generate page number buttons based on totalPages and highlight the current page button to indicate active page */}
            {Array.from({ length: totalPages }, (_, i) => (
              <li
                key={i}
                className={`page-item ${currentPage === i ? "active" : ""}`}
              >
                <button className="page-link" onClick={() => onPageChange(i)}>
                  {i + 1}
                </button>
              </li>
            ))}

            {/* Disable "Next" button on last page to prevent invalid page changes */}
            <li
              className={`page-item ${currentPage === totalPages - 1 ? "disabled" : ""}`}
            >
              <button
                className="page-link"
                onClick={() => onPageChange(currentPage + 1)}
                disabled={currentPage === totalPages - 1}
              >
                Next
              </button>
            </li>
          </ul>
        </nav>
      )}
    </div>
  );
}
export default ReviewList;
