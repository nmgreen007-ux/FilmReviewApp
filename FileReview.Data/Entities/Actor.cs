namespace FileReview.Data.Entities;

public class Actor
{
    public int ActorId { get; set; }
    public required string Name { get; set; }

    public ICollection<CastMember> CastMembers { get; set; } = new List<CastMember>();
}
