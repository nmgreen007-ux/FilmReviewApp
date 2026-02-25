using FileReview.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileReview.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Film> Films => Set<Film>();
    public DbSet<Actor> Actors => Set<Actor>();
    public DbSet<CastMember> CastMembers => Set<CastMember>();
    public DbSet<Review> Reviews => Set<Review>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Film configuration
        modelBuilder.Entity<Film>()
            .HasMany(f => f.Reviews)
            .WithOne(r => r.Film)
            .HasForeignKey(r => r.FilmId);

        modelBuilder.Entity<Film>()
            .HasMany(f => f.CastMembers)
            .WithOne(c => c.Film)
            .HasForeignKey(c => c.FilmId);

        // CastMember configuration
        modelBuilder.Entity<CastMember>()
            .HasOne(c => c.Actor)
            .WithMany(a => a.CastMembers)
            .HasForeignKey(c => c.ActorId);

        // Seed sample data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        var actor1 = new Actor { ActorId = 1, Name = "Emilio Estevez" };
        var actor2 = new Actor { ActorId = 2, Name = "Craig Sheffer" };

        var film = new Film
        {
            FilmId = 1,
            Title = "That was then this is now",
            PosterUrl = "https://image.tmdb.org/t/p/original/cIKmkkgF2Fo5RIZIo7fgpoMp5t6.jpg",
            PlotSummary = "The film follows two close, quasi‑brother teenagers—Mark Jennings and Bryon Douglas—whose lives begin to diverge as Bryon grows more responsible while Mark spirals into jealousy, violence, and drug dealing. Their friendship collapses after Mark’s actions lead to tragedy, forcing Bryon to choose between loyalty and doing what’s right.",
            AverageRanking = 0m
        };

        var castMember1 = new CastMember { CastMemberId = 1, FilmId = 1, ActorId = 1 };
        var castMember2 = new CastMember { CastMemberId = 2, FilmId = 1, ActorId = 2 };

        modelBuilder.Entity<Actor>().HasData(actor1, actor2);
        modelBuilder.Entity<Film>().HasData(film);
        modelBuilder.Entity<CastMember>().HasData(castMember1, castMember2);
    }
}
