using FilmReview.Core.Dtos;
using FilmReview.Core.Services;
using FilmReview.Data.Entities;
using FilmReview.Data.Repositories;
using Moq;

namespace FilmReview.Tests.Services;

public class ReviewsServiceTests
{
    private readonly Mock<IReviewRepository> _reviewRepositoryMock;
    private readonly ReviewsService _reviewsService;

    public ReviewsServiceTests()
    {
        _reviewRepositoryMock = new Mock<IReviewRepository>();
        _reviewsService = new ReviewsService(_reviewRepositoryMock.Object);
    }

    #region GetReviewsAsync Tests

    [Fact]
    public async Task GetReviewsAsync_WithValidFilmId_ReturnsReviewsListDto()
    {
        // Arrange
        int filmId = 1;
        int page = 0;
        var reviews = new List<Review>
        {
            new Review 
            { 
                ReviewId = 1, 
                Note = "Great movie!", 
                Ranking = 5, 
                DisplayName = "John", 
                SubmittedAt = DateTime.UtcNow 
            },
            new Review 
            { 
                ReviewId = 2, 
                Note = "Not bad", 
                Ranking = 3, 
                DisplayName = "Jane", 
                SubmittedAt = DateTime.UtcNow 
            }
        };
        int totalCount = 2;

        _reviewRepositoryMock.Setup(r => r.FilmExistsAsync(filmId))
            .ReturnsAsync(true);
        _reviewRepositoryMock.Setup(r => r.GetReviewsAsync(filmId, page, 10))
            .ReturnsAsync((reviews, totalCount));

        // Act
        var result = await _reviewsService.GetReviewsAsync(filmId, page);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Reviews.Count);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(0, result.Page);
        Assert.Equal(1, result.TotalPages);
        Assert.Equal("Great movie!", result.Reviews[0].Note);
        Assert.Equal(5, result.Reviews[0].Ranking);
        Assert.Equal("John", result.Reviews[0].DisplayName);
        Assert.Equal("Not bad", result.Reviews[1].Note);
        Assert.Equal(3, result.Reviews[1].Ranking);
        Assert.Equal("Jane", result.Reviews[1].DisplayName);
    }

    [Fact]
    public async Task GetReviewsAsync_WithValidFilmIdAndMultiplePages_CalculatesTotalPagesCorrectly()
    {
        // Arrange
        int filmId = 1;
        int page = 0;
        var reviews = Enumerable.Range(1, 10)
            .Select(i => new Review 
            { 
                ReviewId = i, 
                Note = $"Review {i}", 
                Ranking = 5, 
                DisplayName = $"User{i}", 
                SubmittedAt = DateTime.UtcNow 
            })
            .ToList();
        int totalCount = 25; // Will result in 3 pages

        _reviewRepositoryMock.Setup(r => r.FilmExistsAsync(filmId))
            .ReturnsAsync(true);
        _reviewRepositoryMock.Setup(r => r.GetReviewsAsync(filmId, page, 10))
            .ReturnsAsync((reviews, totalCount));

        // Act
        var result = await _reviewsService.GetReviewsAsync(filmId, page);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.TotalPages);
        Assert.Equal(25, result.TotalCount);
    }

    [Fact]
    public async Task GetReviewsAsync_WithNonExistentFilm_ReturnsNull()
    {
        // Arrange
        int filmId = 999;

        _reviewRepositoryMock.Setup(r => r.FilmExistsAsync(filmId))
            .ReturnsAsync(false);

        // Act
        var result = await _reviewsService.GetReviewsAsync(filmId);

        // Assert
        Assert.Null(result);
        _reviewRepositoryMock.Verify(r => r.GetReviewsAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), 
            Times.Never);
    }

    [Fact]
    public async Task GetReviewsAsync_WithValidFilmIdAndNoReviews_ReturnsEmptyList()
    {
        // Arrange
        int filmId = 1;
        int page = 0;
        var reviews = new List<Review>();
        int totalCount = 0;

        _reviewRepositoryMock.Setup(r => r.FilmExistsAsync(filmId))
            .ReturnsAsync(true);
        _reviewRepositoryMock.Setup(r => r.GetReviewsAsync(filmId, page, 10))
            .ReturnsAsync((reviews, totalCount));

        // Act
        var result = await _reviewsService.GetReviewsAsync(filmId, page);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Reviews);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(0, result.TotalPages);
    }

    [Fact]
    public async Task GetReviewsAsync_WithPagination_PassesCorrectPageToRepository()
    {
        // Arrange
        int filmId = 1;
        int page = 2;
        var reviews = new List<Review>();

        _reviewRepositoryMock.Setup(r => r.FilmExistsAsync(filmId))
            .ReturnsAsync(true);
        _reviewRepositoryMock.Setup(r => r.GetReviewsAsync(filmId, page, 10))
            .ReturnsAsync((reviews, 0));

        // Act
        await _reviewsService.GetReviewsAsync(filmId, page);

        // Assert
        _reviewRepositoryMock.Verify(r => r.GetReviewsAsync(filmId, page, 10), Times.Once);
    }

    #endregion

    #region SubmitReviewAsync Tests

    [Fact]
    public async Task SubmitReviewAsync_WithValidFilmIdAndReviewData_ReturnsTrue()
    {
        // Arrange
        int filmId = 1;
        var createReviewDto = new CreateReviewDto
        {
            Note = "Excellent film!",
            Ranking = 5,
            DisplayName = "Alice"
        };
        var review = new Review 
        { 
            ReviewId = 1, 
            Note = "Excellent film!", 
            Ranking = 5, 
            DisplayName = "Alice" 
        };

        _reviewRepositoryMock.Setup(r => r.FilmExistsAsync(filmId))
            .ReturnsAsync(true);
        _reviewRepositoryMock.Setup(r => r.CreateReviewAsync(filmId, It.IsAny<Review>()))
            .ReturnsAsync(review);

        // Act
        var result = await _reviewsService.SubmitReviewAsync(filmId, createReviewDto);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task SubmitReviewAsync_WithValidFilmIdAndNullDisplayName_ReturnsTrue()
    {
        // Arrange
        int filmId = 1;
        var createReviewDto = new CreateReviewDto
        {
            Note = "Good movie",
            Ranking = 4,
            DisplayName = null
        };
        var review = new Review 
        { 
            ReviewId = 1, 
            Note = "Good movie", 
            Ranking = 4, 
            DisplayName = null 
        };

        _reviewRepositoryMock.Setup(r => r.FilmExistsAsync(filmId))
            .ReturnsAsync(true);
        _reviewRepositoryMock.Setup(r => r.CreateReviewAsync(filmId, It.IsAny<Review>()))
            .ReturnsAsync(review);

        // Act
        var result = await _reviewsService.SubmitReviewAsync(filmId, createReviewDto);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task SubmitReviewAsync_WithNonExistentFilm_ReturnsFalse()
    {
        // Arrange
        int filmId = 999;
        var createReviewDto = new CreateReviewDto
        {
            Note = "Review text",
            Ranking = 5,
            DisplayName = "User"
        };

        _reviewRepositoryMock.Setup(r => r.FilmExistsAsync(filmId))
            .ReturnsAsync(false);

        // Act
        var result = await _reviewsService.SubmitReviewAsync(filmId, createReviewDto);

        // Assert
        Assert.False(result);
        _reviewRepositoryMock.Verify(r => r.CreateReviewAsync(It.IsAny<int>(), It.IsAny<Review>()), 
            Times.Never);
    }

    [Fact]
    public async Task SubmitReviewAsync_WhenRepositoryReturnsNull_ReturnsFalse()
    {
        // Arrange
        int filmId = 1;
        var createReviewDto = new CreateReviewDto
        {
            Note = "Review text",
            Ranking = 5,
            DisplayName = "User"
        };

        _reviewRepositoryMock.Setup(r => r.FilmExistsAsync(filmId))
            .ReturnsAsync(true);
        _reviewRepositoryMock.Setup(r => r.CreateReviewAsync(filmId, It.IsAny<Review>()))
            .ReturnsAsync((Review?)null);

        // Act
        var result = await _reviewsService.SubmitReviewAsync(filmId, createReviewDto);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task SubmitReviewAsync_PassesCorrectDataToRepository()
    {
        // Arrange
        int filmId = 1;
        var createReviewDto = new CreateReviewDto
        {
            Note = "Test review",
            Ranking = 4,
            DisplayName = "TestUser"
        };
        var review = new Review 
        { 
            ReviewId = 1, 
            Note = "Test review", 
            Ranking = 4, 
            DisplayName = "TestUser" 
        };

        _reviewRepositoryMock.Setup(r => r.FilmExistsAsync(filmId))
            .ReturnsAsync(true);
        _reviewRepositoryMock.Setup(r => r.CreateReviewAsync(filmId, It.IsAny<Review>()))
            .ReturnsAsync(review);

        // Act
        await _reviewsService.SubmitReviewAsync(filmId, createReviewDto);

        // Assert
        _reviewRepositoryMock.Verify(r => r.CreateReviewAsync(filmId, It.Is<Review>(rv =>
            rv.Note == "Test review" &&
            rv.Ranking == 4 &&
            rv.DisplayName == "TestUser"
        )), Times.Once);
    }

    #endregion
}
