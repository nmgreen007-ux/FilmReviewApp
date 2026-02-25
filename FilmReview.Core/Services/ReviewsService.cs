using FilmReview.Core.Dtos;
using FilmReview.Data.Entities;
using FilmReview.Data.Repositories;

namespace FilmReview.Core.Services;

public class ReviewsService : IReviewsService
{
    private readonly IReviewRepository _reviewRepository;
    private const int PageSize = 10;

    public ReviewsService(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<ReviewsListDto?> GetReviewsAsync(int filmId, int page = 0)
    {
        var filmExists = await _reviewRepository.FilmExistsAsync(filmId);
        if (!filmExists)
            return null;

        var (reviews, totalCount) = await _reviewRepository.GetReviewsAsync(filmId, page, PageSize);

        var totalPages = (totalCount + PageSize - 1) / PageSize;

        return new ReviewsListDto
        {
            Reviews = reviews
                .Select(r => new ReviewDto
                {
                    ReviewId = r.ReviewId,
                    Note = r.Note,
                    Ranking = r.Ranking,
                    DisplayName = r.DisplayName,
                    SubmittedAt = r.SubmittedAt
                })
                .ToList(),
            TotalCount = totalCount,
            Page = page,
            TotalPages = totalPages
        };
    }

    public async Task<List<ReviewDataDto>> GetReviewDataAsync(int filmId)
    {
        var filmExists = await _reviewRepository.FilmExistsAsync(filmId);
        if (!filmExists)
            return new List<ReviewDataDto>();

        var reviews = await _reviewRepository.GetAllReviewsAsync(filmId);

        return reviews
            .Select(r => new ReviewDataDto
            {
                Note = r.Note,
                Ranking = r.Ranking
            })
            .ToList();
    }

    public async Task<bool> SubmitReviewAsync(int filmId, CreateReviewDto createReviewDto)
    {
        var filmExists = await _reviewRepository.FilmExistsAsync(filmId);
        if (!filmExists)
            return false;

        var review = new Review
        {
            Note = createReviewDto.Note,
            Ranking = createReviewDto.Ranking,
            DisplayName = createReviewDto.DisplayName ?? "Anonymous",
            SubmittedAt = DateTime.UtcNow
        };

        var result = await _reviewRepository.CreateReviewAsync(filmId, review);
        return result != null;
    }
}
