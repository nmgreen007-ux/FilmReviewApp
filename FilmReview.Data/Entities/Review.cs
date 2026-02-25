namespace FilmReview.Data.Entities;

public class Review
{
    public int ReviewId { get; set; }
    public int FilmId { get; set; }
    public required string Note { get; set; }
    public int Ranking { get; set; }
    public string DisplayName { get; set; } = "Anonymous";
    public DateTime SubmittedAt { get; set; }

    public Film? Film { get; set; }
}
