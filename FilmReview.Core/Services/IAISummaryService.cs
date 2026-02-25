namespace FilmReview.Core.Services;

/// <summary>
/// Service for generating AI-powered summaries of films
/// </summary>
public interface IAISummaryService
{
    /// <summary>
    /// Generate an AI summary for a film based on its plot and title
    /// </summary>
    /// <param name="filmTitle">The title of the film</param>
    /// <param name="plotSummary">The plot summary of the film</param>
    /// <returns>An AI-generated summary of the film, or null if generation fails</returns>
    Task<string?> GenerateFilmSummaryAsync(string filmTitle, string plotSummary);
}
