namespace FilmReview.Core.Dtos;

/// <summary>
/// Data transfer object for review information used in film statistics calculations
/// </summary>
public class ReviewDataDto
{
    /// <summary>
    /// The review note/text
    /// </summary>
    public required string Note { get; set; }
    
    /// <summary>
    /// The review ranking/rating
    /// </summary>
    public int Ranking { get; set; }
}
