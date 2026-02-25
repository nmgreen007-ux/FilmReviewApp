import type { ReviewDto } from "../types/index";

interface ReviewCardProps {
  review: ReviewDto;
}

function ReviewCard({ review }: ReviewCardProps) {
  const formatDate = (dateString: string): string => {
    const date = new Date(dateString);
    return date.toLocaleDateString("en-UK", {
      year: "numeric",
      month: "short",
      day: "numeric",
    });
  };

  {
    /* use unicode stars to visually represent the ranking, with filled stars for the rating and empty stars for the remaining out of 10 */
  }
  const renderStars = (ranking: number): string => {
    return "★".repeat(ranking) + "☆".repeat(10 - ranking);
  };

  return (
    <div key={review.reviewId} className="review-card">
      <div className="review-header">
        <div>
          <p className="review-author">{review.displayName || "Anonymous"}</p>
          <p className="review-date">{formatDate(review.submittedAt)}</p>
        </div>
        <div className="review-rating">
          <span className="stars">{renderStars(review.ranking)}</span>
          <span className="rating-number">{review.ranking}/10</span>
        </div>
      </div>
      <p className="review-note">{review.note}</p>
    </div>
  );
}

export default ReviewCard;
