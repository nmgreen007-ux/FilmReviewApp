import { useState, type ComponentProps } from "react";
import type { CreateReviewDto } from "../types/index";
import { submitReview } from "../api/client";

type FormSubmitHandler = NonNullable<ComponentProps<"form">["onSubmit"]>;

interface ReviewFormProps {
  filmId: number;
  onSubmitSuccess: () => void;
}

function ReviewForm({ filmId, onSubmitSuccess }: ReviewFormProps) {
  const [note, setNote] = useState("");
  const [ranking, setRanking] = useState(5);
  const [displayName, setDisplayName] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Handle form submission and as prevent form submittion, validate inputs, call API, and notify parent on success
  const handleSubmit: FormSubmitHandler = async (e) => {
    e.preventDefault();
    setError(null);

    if (!note.trim()) {
      setError("Review note is required");
      return;
    }

    if (ranking < 1 || ranking > 10) {
      setError("Ranking must be between 1 and 10");
      return;
    }

    setIsSubmitting(true);

    try {
      const reviewData: CreateReviewDto = {
        note: note.trim(),
        ranking: parseInt(String(ranking), 10),
        displayName: displayName.trim() || undefined,
      };

      await submitReview(filmId, reviewData);

      // Reset form
      setNote("");
      setRanking(5);
      setDisplayName("");

      // Notify parent to refresh
      onSubmitSuccess();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to submit review");
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="review-form-container">
      <h3>Submit a Review</h3>
      <form onSubmit={handleSubmit}>
        <div className="form-group">
          <label htmlFor="displayName">Name (optional)</label>
          <input
            id="displayName"
            type="text"
            value={displayName}
            onChange={(e) => setDisplayName(e.target.value)}
            placeholder="Leave blank for Anonymous"
            maxLength={100}
          />
        </div>

        <div className="form-group">
          <label htmlFor="ranking">Rating</label>
          <div className="ranking-input">
            <input
              id="ranking"
              type="range"
              min="1"
              max="10"
              value={ranking}
              onChange={(e) => setRanking(Number(e.target.value))}
            />
            <span className="ranking-value">{ranking}/10</span>
          </div>
        </div>

        <div className="form-group">
          <label htmlFor="note">Review</label>
          <textarea
            id="note"
            value={note}
            onChange={(e) => setNote(e.target.value)}
            placeholder="What did you think of this film?"
            rows={5}
            maxLength={1000}
          />
        </div>
        {error && <div className="alert alert-danger">{error}</div>}

        <button
          type="submit"
          disabled={isSubmitting}
          className="btn btn-primary"
        >
          {isSubmitting ? "Submitting..." : "Submit Review"}
        </button>
      </form>
    </div>
  );
}
export default ReviewForm;
