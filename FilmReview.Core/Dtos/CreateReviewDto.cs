namespace FilmReview.Core.Dtos;

public class CreateReviewDto
{
    public required string Note { get; set; }
    public int Ranking { get; set; }
    public string? DisplayName { get; set; }
}
