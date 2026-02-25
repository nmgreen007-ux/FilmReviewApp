namespace FileReview.Core.Dtos;

public class FilmDetailDto
{
    public int FilmId { get; set; }
    public required string Title { get; set; }
    public required string PosterUrl { get; set; }
    public required string PlotSummary { get; set; }
    public decimal AverageRanking { get; set; }
    public string? AiSummary { get; set; }
    public List<ActorDto> CastMembers { get; set; } = new();
}
