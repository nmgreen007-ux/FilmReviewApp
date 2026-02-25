namespace FilmReview.Core.Dtos;

public class ReviewsListDto
{
    public required List<ReviewDto> Reviews { get; set; }
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int TotalPages { get; set; }
}
