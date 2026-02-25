namespace FilmReview.Core.Dtos;

public class ReviewDto
{
    public int ReviewId { get; set; }
    public required string Note { get; set; }
    public int Ranking { get; set; }
    public string? DisplayName { get; set; }
    public DateTime SubmittedAt { get; set; }
}
