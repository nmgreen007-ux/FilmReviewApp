using FilmReview.Core.Dtos;
using FilmReview.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace FilmReview.API.Controllers;

[ApiController]
[Route("api/films/{filmId}/reviews")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewsService _reviewsService;
    private readonly IFilmService _filmService;

    public ReviewsController(IReviewsService reviewsService, IFilmService filmService)
    {
        _reviewsService = reviewsService;
        _filmService = filmService;
    }

    /// <summary>
    /// Get paginated reviews for a film, ordered newest first
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ReviewsListDto>> GetReviews(int filmId, [FromQuery] int page = 0)
    {
        var reviews = await _reviewsService.GetReviewsAsync(filmId, page);

        if (reviews == null)
            return NotFound(new { detail = $"Film with ID {filmId} was not found." });

        return Ok(reviews);
    }

    /// <summary>
    /// Submit a new review for a film
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> SubmitReview(int filmId, [FromBody] CreateReviewDto createReviewDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var success = await _reviewsService.SubmitReviewAsync(filmId, createReviewDto);

        if (!success)
            return NotFound(new { detail = $"Film with ID {filmId} was not found." });

        // Get review data (notes and rankings)
        var reviewData = await _reviewsService.GetReviewDataAsync(filmId);

        // Update film statistics (FilmService calculates average and generates AI summary)
        await _filmService.UpdateFilmStatsAsync(filmId, reviewData);

        return Created($"/api/films/{filmId}/reviews", new { });
    }
}

