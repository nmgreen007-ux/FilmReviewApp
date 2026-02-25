using FileReview.Core.Dtos;

namespace FileReview.Core.Services;

public interface IFilmService
{
    Task<FilmDetailDto?> GetFilmAsync(int filmId);

    /// <summary>
    /// Update film statistics including AI summary and average ranking
    /// </summary>
    /// <param name="filmId">The film ID</param>
    /// <param name="reviewData">Review data containing notes and rankings</param>
    /// <returns>True if update was successful, false otherwise</returns>
    Task<bool> UpdateFilmStatsAsync(int filmId, List<ReviewDataDto> reviewData);
}

