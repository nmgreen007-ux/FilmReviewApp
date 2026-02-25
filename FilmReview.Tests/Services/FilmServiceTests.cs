using FileReview.Core.Dtos;
using FileReview.Core.Services;
using FileReview.Data.Entities;
using FileReview.Data.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace FilmReview.Tests.Services;

public class FilmServiceTests
{
    private readonly Mock<IFilmRepository> _filmRepositoryMock;
    private readonly Mock<IAISummaryService> _aiSummaryServiceMock;
    private readonly Mock<ILogger<FilmService>> _loggerMock;
    private readonly FilmService _filmService;

    public FilmServiceTests()
    {
        _filmRepositoryMock = new Mock<IFilmRepository>();
        _aiSummaryServiceMock = new Mock<IAISummaryService>();
        _loggerMock = new Mock<ILogger<FilmService>>();

        _filmService = new FilmService(
            _filmRepositoryMock.Object,
            _aiSummaryServiceMock.Object,
            _loggerMock.Object);
    }

    #region GetFilmAsync Tests

    [Fact]
    public async Task GetFilmAsync_WithValidFilmId_ReturnsFilmDetailDto()
    {
        // Arrange
        int filmId = 1;
        var film = new Film
        {
            FilmId = filmId,
            Title = "Inception",
            PosterUrl = "https://example.com/inception.jpg",
            PlotSummary = "A thief who steals corporate secrets through dream-sharing technology.",
            AverageRanking = 8.8m,
            AiSummary = "A mind-bending sci-fi thriller about dreams and reality.",
            CastMembers = new List<CastMember>
            {
                new CastMember
                {
                    CastMemberId = 1,
                    ActorId = 1,
                    FilmId = filmId,
                    Actor = new Actor { ActorId = 1, Name = "Leonardo DiCaprio" }
                },
                new CastMember
                {
                    CastMemberId = 2,
                    ActorId = 2,
                    FilmId = filmId,
                    Actor = new Actor { ActorId = 2, Name = "Marion Cotillard" }
                }
            }
        };

        _filmRepositoryMock.Setup(r => r.GetFilmByIdAsync(filmId))
            .ReturnsAsync(film);

        // Act
        var result = await _filmService.GetFilmAsync(filmId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(filmId, result.FilmId);
        Assert.Equal("Inception", result.Title);
        Assert.Equal("https://example.com/inception.jpg", result.PosterUrl);
        Assert.Equal("A thief who steals corporate secrets through dream-sharing technology.", result.PlotSummary);
        Assert.Equal(8.8m, result.AverageRanking);
        Assert.Equal("A mind-bending sci-fi thriller about dreams and reality.", result.AiSummary);
    }

    [Fact]
    public async Task GetFilmAsync_WithValidFilmId_MapsCastMembersCorrectly()
    {
        // Arrange
        int filmId = 1;
        var film = new Film
        {
            FilmId = filmId,
            Title = "The Matrix",
            PosterUrl = "https://example.com/matrix.jpg",
            PlotSummary = "A computer hacker learns about the true nature of his reality.",
            AverageRanking = 8.7m,
            AiSummary = "A groundbreaking sci-fi action film.",
            CastMembers = new List<CastMember>
            {
                new CastMember
                {
                    CastMemberId = 1,
                    ActorId = 1,
                    FilmId = filmId,
                    Actor = new Actor { ActorId = 1, Name = "Keanu Reeves" }
                },
                new CastMember
                {
                    CastMemberId = 2,
                    ActorId = 2,
                    FilmId = filmId,
                    Actor = new Actor { ActorId = 2, Name = "Laurence Fishburne" }
                },
                new CastMember
                {
                    CastMemberId = 3,
                    ActorId = 3,
                    FilmId = filmId,
                    Actor = new Actor { ActorId = 3, Name = "Carrie-Anne Moss" }
                }
            }
        };

        _filmRepositoryMock.Setup(r => r.GetFilmByIdAsync(filmId))
            .ReturnsAsync(film);

        // Act
        var result = await _filmService.GetFilmAsync(filmId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.CastMembers.Count);
        Assert.Equal("Keanu Reeves", result.CastMembers[0].Name);
        Assert.Equal(1, result.CastMembers[0].ActorId);
        Assert.Equal("Laurence Fishburne", result.CastMembers[1].Name);
        Assert.Equal(2, result.CastMembers[1].ActorId);
        Assert.Equal("Carrie-Anne Moss", result.CastMembers[2].Name);
        Assert.Equal(3, result.CastMembers[2].ActorId);
    }

    [Fact]
    public async Task GetFilmAsync_WithValidFilmIdAndNoCastMembers_ReturnsEmptyCastMembersList()
    {
        // Arrange
        int filmId = 1;
        var film = new Film
        {
            FilmId = filmId,
            Title = "Unknown Film",
            PosterUrl = "https://example.com/unknown.jpg",
            PlotSummary = "A film with no cast information.",
            AverageRanking = 0m,
            AiSummary = null,
            CastMembers = new List<CastMember>()
        };

        _filmRepositoryMock.Setup(r => r.GetFilmByIdAsync(filmId))
            .ReturnsAsync(film);

        // Act
        var result = await _filmService.GetFilmAsync(filmId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.CastMembers);
    }

    [Fact]
    public async Task GetFilmAsync_WithValidFilmIdAndNullAiSummary_ReturnsNullAiSummary()
    {
        // Arrange
        int filmId = 1;
        var film = new Film
        {
            FilmId = filmId,
            Title = "Test Film",
            PosterUrl = "https://example.com/test.jpg",
            PlotSummary = "A test film.",
            AverageRanking = 5.0m,
            AiSummary = null,
            CastMembers = new List<CastMember>()
        };

        _filmRepositoryMock.Setup(r => r.GetFilmByIdAsync(filmId))
            .ReturnsAsync(film);

        // Act
        var result = await _filmService.GetFilmAsync(filmId);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.AiSummary);
    }

    [Fact]
    public async Task GetFilmAsync_WithNonExistentFilm_ReturnsNull()
    {
        // Arrange
        int filmId = 999;

        _filmRepositoryMock.Setup(r => r.GetFilmByIdAsync(filmId))
            .ReturnsAsync((Film?)null);

        // Act
        var result = await _filmService.GetFilmAsync(filmId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetFilmAsync_CallsRepositoryWithCorrectFilmId()
    {
        // Arrange
        int filmId = 42;

        _filmRepositoryMock.Setup(r => r.GetFilmByIdAsync(filmId))
            .ReturnsAsync((Film?)null);

        // Act
        await _filmService.GetFilmAsync(filmId);

        // Assert
        _filmRepositoryMock.Verify(r => r.GetFilmByIdAsync(filmId), Times.Once);
    }

    [Fact]
    public async Task GetFilmAsync_WithZeroAverageRanking_ReturnsZeroRanking()
    {
        // Arrange
        int filmId = 1;
        var film = new Film
        {
            FilmId = filmId,
            Title = "New Film",
            PosterUrl = "https://example.com/new.jpg",
            PlotSummary = "A newly released film.",
            AverageRanking = 0m,
            AiSummary = "No summary yet.",
            CastMembers = new List<CastMember>()
        };

        _filmRepositoryMock.Setup(r => r.GetFilmByIdAsync(filmId))
            .ReturnsAsync(film);

        // Act
        var result = await _filmService.GetFilmAsync(filmId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0m, result.AverageRanking);
    }

    [Fact]
    public async Task GetFilmAsync_WithMaxAverageRanking_ReturnsMaxRanking()
    {
        // Arrange
        int filmId = 1;
        var film = new Film
        {
            FilmId = filmId,
            Title = "Perfect Film",
            PosterUrl = "https://example.com/perfect.jpg",
            PlotSummary = "The best film ever.",
            AverageRanking = 10m,
            AiSummary = "Perfect rating.",
            CastMembers = new List<CastMember>()
        };

        _filmRepositoryMock.Setup(r => r.GetFilmByIdAsync(filmId))
            .ReturnsAsync(film);

        // Act
        var result = await _filmService.GetFilmAsync(filmId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10m, result.AverageRanking);
    }

    [Fact]
    public async Task GetFilmAsync_WithSingleCastMember_ReturnsSingleActor()
    {
        // Arrange
        int filmId = 1;
        var film = new Film
        {
            FilmId = filmId,
            Title = "Solo",
            PosterUrl = "https://example.com/solo.jpg",
            PlotSummary = "A film with one actor.",
            AverageRanking = 6.5m,
            AiSummary = "Solo performance.",
            CastMembers = new List<CastMember>
            {
                new CastMember
                {
                    CastMemberId = 1,
                    ActorId = 1,
                    FilmId = filmId,
                    Actor = new Actor { ActorId = 1, Name = "Tom Hanks" }
                }
            }
        };

        _filmRepositoryMock.Setup(r => r.GetFilmByIdAsync(filmId))
            .ReturnsAsync(film);

        // Act
        var result = await _filmService.GetFilmAsync(filmId);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.CastMembers);
        Assert.Equal("Tom Hanks", result.CastMembers[0].Name);
    }

    #endregion

    #region UpdateFilmStatsAsync Tests

    [Fact]
    public async Task UpdateFilmStatsAsync_WithMultipleReviews_CalculatesCorrectAverageRanking()
    {
        // Arrange
        int filmId = 1;
        var film = new Film
        {
            FilmId = filmId,
            Title = "Test Film",
            PosterUrl = "https://example.com/test.jpg",
            PlotSummary = "A test film.",
            AverageRanking = 0m,
            AiSummary = null
        };

        var reviewData = new List<ReviewDataDto>
        {
            new ReviewDataDto { Note = "Great!", Ranking = 10 },
            new ReviewDataDto { Note = "Good", Ranking = 8 },
            new ReviewDataDto { Note = "Average", Ranking = 5 }
        };

        _filmRepositoryMock.Setup(r => r.GetFilmByIdAsync(filmId))
            .ReturnsAsync(film);

        _filmRepositoryMock.Setup(r => r.UpdateFilmAsync(It.IsAny<Film>()))
            .ReturnsAsync(film);

        // Act
        var result = await _filmService.UpdateFilmStatsAsync(filmId, reviewData);

        // Assert
        Assert.True(result);
        var expectedAverage = (decimal)(10 + 8 + 5) / 3; // 7.666...
        Assert.Equal(expectedAverage, film.AverageRanking, 10); // 10 decimal places precision
    }

    [Fact]
    public async Task UpdateFilmStatsAsync_WithSingleReview_CalculatesCorrectAverageRanking()
    {
        // Arrange
        int filmId = 1;
        var film = new Film
        {
            FilmId = filmId,
            Title = "Test Film",
            PosterUrl = "https://example.com/test.jpg",
            PlotSummary = "A test film.",
            AverageRanking = 0m,
            AiSummary = null
        };

        var reviewData = new List<ReviewDataDto>
        {
            new ReviewDataDto { Note = "Excellent!", Ranking = 9 }
        };

        _filmRepositoryMock.Setup(r => r.GetFilmByIdAsync(filmId))
            .ReturnsAsync(film);

        _filmRepositoryMock.Setup(r => r.UpdateFilmAsync(It.IsAny<Film>()))
            .ReturnsAsync(film);

        // Act
        var result = await _filmService.UpdateFilmStatsAsync(filmId, reviewData);

        // Assert
        Assert.True(result);
        Assert.Equal(9m, film.AverageRanking);
    }

    [Fact]
    public async Task UpdateFilmStatsAsync_WithEmptyReviews_SetsAverageRankingToZero()
    {
        // Arrange
        int filmId = 1;
        var film = new Film
        {
            FilmId = filmId,
            Title = "Test Film",
            PosterUrl = "https://example.com/test.jpg",
            PlotSummary = "A test film.",
            AverageRanking = 5m,
            AiSummary = "Some summary"
        };

        var reviewData = new List<ReviewDataDto>();

        _filmRepositoryMock.Setup(r => r.GetFilmByIdAsync(filmId))
            .ReturnsAsync(film);

        _filmRepositoryMock.Setup(r => r.UpdateFilmAsync(It.IsAny<Film>()))
            .ReturnsAsync(film);

        // Act
        var result = await _filmService.UpdateFilmStatsAsync(filmId, reviewData);

        // Assert
        Assert.True(result);
        Assert.Equal(0m, film.AverageRanking);
        Assert.Null(film.AiSummary);
    }

    [Fact]
    public async Task UpdateFilmStatsAsync_WithHighRatings_CalculatesCorrectAverageRanking()
    {
        // Arrange
        int filmId = 1;
        var film = new Film
        {
            FilmId = filmId,
            Title = "Test Film",
            PosterUrl = "https://example.com/test.jpg",
            PlotSummary = "A test film.",
            AverageRanking = 0m,
            AiSummary = null
        };

        var reviewData = new List<ReviewDataDto>
        {
            new ReviewDataDto { Note = "Amazing!", Ranking = 10 },
            new ReviewDataDto { Note = "Perfect!", Ranking = 10 },
            new ReviewDataDto { Note = "Excellent!", Ranking = 9 }
        };

        _filmRepositoryMock.Setup(r => r.GetFilmByIdAsync(filmId))
            .ReturnsAsync(film);

        _filmRepositoryMock.Setup(r => r.UpdateFilmAsync(It.IsAny<Film>()))
            .ReturnsAsync(film);

        // Act
        var result = await _filmService.UpdateFilmStatsAsync(filmId, reviewData);

        // Assert
        Assert.True(result);
        var expectedAverage = (decimal)(10 + 10 + 9) / 3; // 9.666...
        Assert.Equal(expectedAverage, film.AverageRanking, 10); // 10 decimal places precision
    }

    [Fact]
    public async Task UpdateFilmStatsAsync_WithLowRatings_CalculatesCorrectAverageRanking()
    {
        // Arrange
        int filmId = 1;
        var film = new Film
        {
            FilmId = filmId,
            Title = "Test Film",
            PosterUrl = "https://example.com/test.jpg",
            PlotSummary = "A test film.",
            AverageRanking = 0m,
            AiSummary = null
        };

        var reviewData = new List<ReviewDataDto>
        {
            new ReviewDataDto { Note = "Poor", Ranking = 2 },
            new ReviewDataDto { Note = "Bad", Ranking = 1 },
            new ReviewDataDto { Note = "Terrible", Ranking = 1 }
        };

        _filmRepositoryMock.Setup(r => r.GetFilmByIdAsync(filmId))
            .ReturnsAsync(film);

        _filmRepositoryMock.Setup(r => r.UpdateFilmAsync(It.IsAny<Film>()))
            .ReturnsAsync(film);

        // Act
        var result = await _filmService.UpdateFilmStatsAsync(filmId, reviewData);

        // Assert
        Assert.True(result);
        var expectedAverage = (decimal)(2 + 1 + 1) / 3; // 1.333...
        Assert.Equal(expectedAverage, film.AverageRanking, 10); // 10 decimal places precision
    }

    [Fact]
    public async Task UpdateFilmStatsAsync_WithMixedRatings_CalculatesCorrectAverageRanking()
    {
        // Arrange
        int filmId = 1;
        var film = new Film
        {
            FilmId = filmId,
            Title = "Test Film",
            PosterUrl = "https://example.com/test.jpg",
            PlotSummary = "A test film.",
            AverageRanking = 0m,
            AiSummary = null
        };

        var reviewData = new List<ReviewDataDto>
        {
            new ReviewDataDto { Note = "Amazing", Ranking = 10 },
            new ReviewDataDto { Note = "Average", Ranking = 5 },
            new ReviewDataDto { Note = "Poor", Ranking = 2 }
        };

        _filmRepositoryMock.Setup(r => r.GetFilmByIdAsync(filmId))
            .ReturnsAsync(film);

        _filmRepositoryMock.Setup(r => r.UpdateFilmAsync(It.IsAny<Film>()))
            .ReturnsAsync(film);

        // Act
        var result = await _filmService.UpdateFilmStatsAsync(filmId, reviewData);

        // Assert
        Assert.True(result);
        var expectedAverage = (decimal)(10 + 5 + 2) / 3; // 5.666...
        Assert.Equal(expectedAverage, film.AverageRanking, 10); // 10 decimal places precision
    }

    [Fact]
    public async Task UpdateFilmStatsAsync_WithNonExistentFilm_ReturnsFalse()
    {
        // Arrange
        int filmId = 999;
        var reviewData = new List<ReviewDataDto>
        {
            new ReviewDataDto { Note = "Good", Ranking = 8 }
        };

        _filmRepositoryMock.Setup(r => r.GetFilmByIdAsync(filmId))
            .ReturnsAsync((Film?)null);

        // Act
        var result = await _filmService.UpdateFilmStatsAsync(filmId, reviewData);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateFilmStatsAsync_CallsRepositoryUpdateWithUpdatedFilm()
    {
        // Arrange
        int filmId = 1;
        var film = new Film
        {
            FilmId = filmId,
            Title = "Test Film",
            PosterUrl = "https://example.com/test.jpg",
            PlotSummary = "A test film.",
            AverageRanking = 0m,
            AiSummary = null
        };

        var reviewData = new List<ReviewDataDto>
        {
            new ReviewDataDto { Note = "Good", Ranking = 7 }
        };

        _filmRepositoryMock.Setup(r => r.GetFilmByIdAsync(filmId))
            .ReturnsAsync(film);

        _filmRepositoryMock.Setup(r => r.UpdateFilmAsync(It.IsAny<Film>()))
            .ReturnsAsync(film);

        // Act
        await _filmService.UpdateFilmStatsAsync(filmId, reviewData);

        // Assert
        _filmRepositoryMock.Verify(
            r => r.UpdateFilmAsync(It.Is<Film>(f => f.AverageRanking == 7m)),
            Times.Once);
    }

    [Fact]
    public async Task UpdateFilmStatsAsync_CallsAISummaryServiceWithReviewNotes()
    {
        // Arrange
        int filmId = 1;
        var film = new Film
        {
            FilmId = filmId,
            Title = "Test Film",
            PosterUrl = "https://example.com/test.jpg",
            PlotSummary = "A test film.",
            AverageRanking = 0m,
            AiSummary = null
        };

        var reviewData = new List<ReviewDataDto>
        {
            new ReviewDataDto { Note = "Amazing film!", Ranking = 10 },
            new ReviewDataDto { Note = "Brilliant performance", Ranking = 9 }
        };

        _filmRepositoryMock.Setup(r => r.GetFilmByIdAsync(filmId))
            .ReturnsAsync(film);

        _filmRepositoryMock.Setup(r => r.UpdateFilmAsync(It.IsAny<Film>()))
            .ReturnsAsync(film);

        _aiSummaryServiceMock.Setup(s => s.GenerateFilmSummaryAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync("AI generated summary");

        // Act
        await _filmService.UpdateFilmStatsAsync(filmId, reviewData);

        // Assert
        _aiSummaryServiceMock.Verify(
            s => s.GenerateFilmSummaryAsync(
                "Test Film",
                It.Is<string>(notes => notes.Contains("Amazing film!") && notes.Contains("Brilliant performance"))),
            Times.Once);
    }

    #endregion
}
