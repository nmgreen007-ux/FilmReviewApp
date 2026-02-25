using FileReview.Core.Dtos;
using FileReview.Data.Repositories;
using Microsoft.Extensions.Logging;

namespace FileReview.Core.Services;

public class FilmService : IFilmService
{
    private readonly IFilmRepository _filmRepository;
    private readonly IAISummaryService _aiSummaryService;
    private readonly ILogger<FilmService> _logger;

    public FilmService(
        IFilmRepository filmRepository,
        IAISummaryService aiSummaryService,
        ILogger<FilmService> logger)
    {
        _filmRepository = filmRepository;
        _aiSummaryService = aiSummaryService;
        _logger = logger;
    }

    public async Task<FilmDetailDto?> GetFilmAsync(int filmId)
    {
        var film = await _filmRepository.GetFilmByIdAsync(filmId);

        if (film == null)
            return null;

        return new FilmDetailDto
        {
            FilmId = film.FilmId,
            Title = film.Title,
            PosterUrl = film.PosterUrl,
            PlotSummary = film.PlotSummary,
            AverageRanking = film.AverageRanking,
            AiSummary = film.AiSummary,
            CastMembers = film.CastMembers
                .Select(cm => new ActorDto
                {
                    ActorId = cm.Actor!.ActorId,
                    Name = cm.Actor.Name
                })
                .ToList()
        };
    }

    public async Task<bool> UpdateFilmStatsAsync(int filmId, List<ReviewDataDto> reviewData)
    {
        try
        {
            var film = await _filmRepository.GetFilmByIdAsync(filmId);
            if (film == null)
            {
                _logger.LogWarning($"Film with ID {filmId} not found for stats update");
                return false;
            }

            if (reviewData.Count == 0)
            {
                film.AverageRanking = 0;
                film.AiSummary = null;
                _logger.LogInformation($"No reviews found for film {filmId}");
            }
            else
            {
                // Calculate average ranking from review data
                var averageRanking = reviewData.Average(r => r.Ranking);
                film.AverageRanking = (decimal)averageRanking;

                // Generate AI summary from review notes
                var reviewNotes = reviewData.Select(r => r.Note).ToList();
                var combinedNotes = string.Join(" ", reviewNotes);
                var aiSummary = await _aiSummaryService.GenerateFilmSummaryAsync(film.Title, combinedNotes);

                if (!string.IsNullOrEmpty(aiSummary))
                {
                    film.AiSummary = aiSummary;
                    _logger.LogInformation($"Generated AI summary for film {filmId}");
                }
            }

            // Save updated film
            await _filmRepository.UpdateFilmAsync(film);
            _logger.LogInformation($"Updated film statistics for film {filmId}. Average Ranking: {film.AverageRanking}");

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating film stats for film {filmId}");
            return false;
        }
    }
}


