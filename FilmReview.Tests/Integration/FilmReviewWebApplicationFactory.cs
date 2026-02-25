using FileReview.Data;
using FileReview.Data.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace FilmReview.Tests.Integration;

public class FilmReviewWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove all database context and provider related services
            var descriptorsToRemove = services.Where(d => 
                d.ServiceType.IsAssignableFrom(typeof(ApplicationDbContext)) ||
                d.ServiceType == typeof(ApplicationDbContext) ||
                (d.ServiceType.IsGenericType && 
                 d.ServiceType.GetGenericTypeDefinition() == typeof(DbContextOptions<>) &&
                 d.ServiceType.GetGenericArguments()[0] == typeof(ApplicationDbContext)) ||
                d.ServiceType.FullName?.Contains("EntityFrameworkCore") == true).ToList();

            foreach (var descriptor in descriptorsToRemove)
            {
                services.Remove(descriptor);
            }

            // Add an in-memory database for testing with unique name for each host creation
            var dbName = $"FilmReviewTestDb_{Guid.NewGuid()}";
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase(dbName);
            });
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        // Seed test data after host is created
        using var scope = host.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.EnsureCreated();

        SeedTestData(dbContext);

        return host;
    }

    private static void SeedTestData(ApplicationDbContext dbContext)
    {
        // Clear any existing data
        dbContext.Films.RemoveRange(dbContext.Films);
        dbContext.Actors.RemoveRange(dbContext.Actors);
        dbContext.Reviews.RemoveRange(dbContext.Reviews);
        dbContext.CastMembers.RemoveRange(dbContext.CastMembers);
        dbContext.SaveChanges();

        // Add test films
        var film1 = new Film
        {
            FilmId = 1,
            Title = "Inception",
            PosterUrl = "https://example.com/inception.jpg",
            PlotSummary = "A thief who steals corporate secrets through dream-sharing technology.",
            AverageRanking = 8.8m,
            AiSummary = "A mind-bending sci-fi thriller."
        };

        var film2 = new Film
        {
            FilmId = 2,
            Title = "The Matrix",
            PosterUrl = "https://example.com/matrix.jpg",
            PlotSummary = "A computer hacker learns about the true nature of his reality.",
            AverageRanking = 8.7m,
            AiSummary = "A groundbreaking sci-fi action film."
        };

        // Add test actors
        var actor1 = new Actor { ActorId = 1, Name = "Leonardo DiCaprio" };
        var actor2 = new Actor { ActorId = 2, Name = "Keanu Reeves" };

        // Add cast members
        var castMember1 = new CastMember { CastMemberId = 1, FilmId = 1, ActorId = 1 };
        var castMember2 = new CastMember { CastMemberId = 2, FilmId = 2, ActorId = 2 };

        // Add test reviews
        var review1 = new Review
        {
            ReviewId = 1,
            FilmId = 1,
            Note = "Amazing film!",
            Ranking = 5,
            DisplayName = "John",
            SubmittedAt = DateTime.UtcNow.AddDays(-1)
        };

        var review2 = new Review
        {
            ReviewId = 2,
            FilmId = 1,
            Note = "Good movie",
            Ranking = 4,
            DisplayName = "Jane",
            SubmittedAt = DateTime.UtcNow.AddDays(-2)
        };

        dbContext.Films.AddRange(film1, film2);
        dbContext.Actors.AddRange(actor1, actor2);
        dbContext.CastMembers.AddRange(castMember1, castMember2);
        dbContext.Reviews.AddRange(review1, review2);
        dbContext.SaveChanges();
    }
}
