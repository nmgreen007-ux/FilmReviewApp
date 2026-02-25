using FilmReview.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FilmReview.Data.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly ApplicationDbContext _context;
    private const int DefaultPageSize = 10;

    public ReviewRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(List<Review> Reviews, int TotalCount)> GetReviewsAsync(int filmId, int page = 0, int pageSize = 10)
    {
        pageSize = pageSize <= 0 ? DefaultPageSize : pageSize;

        var totalCount = await _context.Reviews
            .CountAsync(r => r.FilmId == filmId);

        var reviews = await _context.Reviews
            .Where(r => r.FilmId == filmId)
            .OrderByDescending(r => r.SubmittedAt)
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (reviews, totalCount);
    }

    public async Task<Review?> CreateReviewAsync(int filmId, Review review)
    {
        review.FilmId = filmId;
        review.SubmittedAt = DateTime.UtcNow;
        review.DisplayName = review.DisplayName?.Trim() ?? null;

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        return review;
    }

    public async Task<bool> FilmExistsAsync(int filmId)
    {
        return await _context.Films.AnyAsync(f => f.FilmId == filmId);
    }

    public async Task<List<Review>> GetAllReviewsAsync(int filmId)
    {
        return await _context.Reviews
            .Where(r => r.FilmId == filmId)
            .OrderByDescending(r => r.SubmittedAt)
            .ToListAsync();
    }
}
