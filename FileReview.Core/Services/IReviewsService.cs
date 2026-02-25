using FileReview.Core.Dtos;

namespace FileReview.Core.Services;

public interface IReviewsService
{
    Task<ReviewsListDto?> GetReviewsAsync(int filmId, int page = 0);
    Task<bool> SubmitReviewAsync(int filmId, CreateReviewDto createReviewDto);

    /// <summary>
    /// Get all review data for a film (notes and rankings for aggregation)
    /// </summary>
    Task<List<ReviewDataDto>> GetReviewDataAsync(int filmId);
}


