namespace FilmReview.Data.Entities;

public class Film
{
    public int FilmId { get; set; }
    public required string Title { get; set; }
    public required string PosterUrl { get; set; }
    public required string PlotSummary { get; set; }
    public decimal AverageRanking { get; set; }
    public string? AiSummary { get; set; }

    public ICollection<CastMember> CastMembers { get; set; } = new List<CastMember>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
