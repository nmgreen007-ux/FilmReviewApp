namespace FilmReview.Data.Entities;

public class CastMember
{
    public int CastMemberId { get; set; }
    public int FilmId { get; set; }
    public int ActorId { get; set; }

    public Film? Film { get; set; }
    public Actor? Actor { get; set; }
}
