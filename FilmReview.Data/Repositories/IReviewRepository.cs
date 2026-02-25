using FilmReview.Data.Entities;

namespace FilmReview.Data.Repositories;

public interface IReviewRepository
{
    Task<(List<Review> Reviews, int TotalCount)> GetReviewsAsync(int filmId, int page = 0, int pageSize = 10);
    Task<Review?> CreateReviewAsync(int filmId, Review review);
    Task<bool> FilmExistsAsync(int filmId);

    /// <summary>
    /// Get all reviews for a film without pagination
    /// </summary>
    Task<List<Review>> GetAllReviewsAsync(int filmId);
}

