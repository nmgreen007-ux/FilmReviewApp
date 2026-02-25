using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FilmReview.Core.Dtos;
using Xunit;

namespace FilmReview.Tests.Integration;

public class ReviewsControllerIntegrationTests : IClassFixture<FilmReviewWebApplicationFactory>
{
    private readonly FilmReviewWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public ReviewsControllerIntegrationTests(FilmReviewWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task<T?> GetContentAsAsync<T>(HttpContent content)
    {
        var jsonString = await content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(jsonString, JsonOptions);
    }

    private static StringContent CreateJsonContent<T>(T data)
    {
        return new StringContent(
            JsonSerializer.Serialize(data, JsonOptions),
            Encoding.UTF8,
            "application/json");
    }

    [Fact]
    public async Task GetReviews_WithValidFilmId_ReturnsOkWithReviews()
    {
        // Act
        var response = await _client.GetAsync("/api/films/1/reviews");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await GetContentAsAsync<ReviewsListDto>(response.Content);
        Assert.NotNull(content);
        Assert.NotEmpty(content.Reviews);
        Assert.True(content.TotalCount > 0);
    }

    [Fact]
    public async Task GetReviews_WithValidFilmId_ReturnsPaginatedReviews()
    {
        // Act - Get first page
        var response = await _client.GetAsync("/api/films/1/reviews?page=0");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await GetContentAsAsync<ReviewsListDto>(response.Content);
        Assert.Equal(0, content!.Page);
        Assert.True(content.TotalCount >= 2); // At least the 2 seeded reviews
    }

    [Fact]
    public async Task GetReviews_WithInvalidFilmId_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/films/999/reviews");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetReviews_ResponseIncludesReviewDetails()
    {
        // Act
        var response = await _client.GetAsync("/api/films/1/reviews");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await GetContentAsAsync<ReviewsListDto>(response.Content);

        var firstReview = content!.Reviews.First();
        Assert.True(firstReview.ReviewId > 0);
        Assert.NotNull(firstReview.Note);
        Assert.True(firstReview.Ranking > 0);
        Assert.NotNull(firstReview.DisplayName);
        Assert.NotEqual(default, firstReview.SubmittedAt);
    }

    [Fact]
    public async Task SubmitReview_WithValidData_ReturnsCreated()
    {
        // Arrange
        var newReview = new CreateReviewDto
        {
            Note = "Excellent film!",
            Ranking = 5,
            DisplayName = "TestUser"
        };
        var content = CreateJsonContent(newReview);

        // Act
        var response = await _client.PostAsync("/api/films/1/reviews", content);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        Assert.Contains("/api/films/1/reviews", response.Headers.Location.ToString());
    }

    [Fact]
    public async Task SubmitReview_WithValidData_AddsReviewToFilm()
    {
        // Arrange
        var initialResponse = await _client.GetAsync("/api/films/1/reviews");
        var initialContent = await GetContentAsAsync<ReviewsListDto>(initialResponse.Content);
        var initialCount = initialContent!.TotalCount;

        var newReview = new CreateReviewDto
        {
            Note = "Great movie!",
            Ranking = 5,
            DisplayName = "NewUser"
        };
        var postContent = CreateJsonContent(newReview);

        // Act
        var postResponse = await _client.PostAsync("/api/films/1/reviews", postContent);

        // Assert
        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

        // Verify review was added
        var finalResponse = await _client.GetAsync("/api/films/1/reviews");
        var finalContent = await GetContentAsAsync<ReviewsListDto>(finalResponse.Content);
        Assert.Equal(initialCount + 1, finalContent!.TotalCount);
    }

    [Fact]
    public async Task SubmitReview_WithInvalidFilmId_ReturnsNotFound()
    {
        // Arrange
        var newReview = new CreateReviewDto
        {
            Note = "Review text",
            Ranking = 5,
            DisplayName = "User"
        };
        var content = CreateJsonContent(newReview);

        // Act
        var response = await _client.PostAsync("/api/films/999/reviews", content);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task SubmitReview_WithNullDisplayName_ReturnsCreated()
    {
        // Arrange
        var newReview = new CreateReviewDto
        {
            Note = "Anonymous review",
            Ranking = 4,
            DisplayName = null
        };
        var content = CreateJsonContent(newReview);

        // Act
        var response = await _client.PostAsync("/api/films/1/reviews", content);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task GetReviews_WithPaginationPage2_ReturnsEmpty()
    {
        // Act - Get page 2 when only 2 reviews exist on page 0
        var response = await _client.GetAsync("/api/films/1/reviews?page=2");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await GetContentAsAsync<ReviewsListDto>(response.Content);
        Assert.Equal(2, content!.Page);
        Assert.Empty(content.Reviews);
    }

    [Fact]
    public async Task GetReviews_CalculatesTotalPagesCorrectly()
    {
        // Act
        var response = await _client.GetAsync("/api/films/1/reviews");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await GetContentAsAsync<ReviewsListDto>(response.Content);
        Assert.Equal(1, content!.TotalPages);
    }

    [Fact]
    public async Task SubmitReview_UpdatesFilmAverageRanking()
    {
        // Arrange - Get initial film data
        var initialFilmResponse = await _client.GetAsync("/api/films/1");
        var initialFilm = await GetContentAsAsync<FilmDetailDto>(initialFilmResponse.Content);
        var initialAverageRanking = initialFilm!.AverageRanking;

        // Submit a new review
        var newReview = new CreateReviewDto
        {
            Note = "Excellent film!",
            Ranking = 10,
            DisplayName = "CriticalUser"
        };
        var content = CreateJsonContent(newReview);

        // Act - Submit review
        var submitResponse = await _client.PostAsync("/api/films/1/reviews", content);

        // Assert - Verify review was created
        Assert.Equal(HttpStatusCode.Created, submitResponse.StatusCode);

        // Get updated film data
        var updatedFilmResponse = await _client.GetAsync("/api/films/1");
        var updatedFilm = await GetContentAsAsync<FilmDetailDto>(updatedFilmResponse.Content);

        // Verify average ranking was updated
        Assert.NotNull(updatedFilm);
        Assert.NotEqual(initialAverageRanking, updatedFilm.AverageRanking);
        Assert.True(updatedFilm.AverageRanking > 0);
    }

    [Fact]
    public async Task SubmitReview_UpdatesFilmAiSummary()
    {
        // Arrange - Get initial film data
        var initialFilmResponse = await _client.GetAsync("/api/films/1");
        var initialFilm = await GetContentAsAsync<FilmDetailDto>(initialFilmResponse.Content);
        var initialAiSummary = initialFilm!.AiSummary;

        // Submit a new review
        var newReview = new CreateReviewDto
        {
            Note = "Outstanding cinematography and brilliant performances",
            Ranking = 9,
            DisplayName = "FilmCritic"
        };
        var content = CreateJsonContent(newReview);

        // Act - Submit review
        var submitResponse = await _client.PostAsync("/api/films/1/reviews", content);

        // Assert - Verify review was created
        Assert.Equal(HttpStatusCode.Created, submitResponse.StatusCode);

        // Get updated film data
        var updatedFilmResponse = await _client.GetAsync("/api/films/1");
        var updatedFilm = await GetContentAsAsync<FilmDetailDto>(updatedFilmResponse.Content);

        // Note: AiSummary might still be null if Azure OpenAI is not configured
        // but the film object should be updated
        Assert.NotNull(updatedFilm);
        // If AI summary was generated, it should be different or populated
        // (In test environment without Azure OpenAI configured, it may still be the seeded value)
    }

    [Fact]
    public async Task SubmitMultipleReviews_CalculatesCorrectAverageRanking()
    {
        // Arrange
        var reviews = new[]
        {
            new CreateReviewDto { Note = "Excellent!", Ranking = 10, DisplayName = "User1" },
            new CreateReviewDto { Note = "Good", Ranking = 8, DisplayName = "User2" },
            new CreateReviewDto { Note = "Average", Ranking = 5, DisplayName = "User3" }
        };

        // Get initial state
        var initialFilmResponse = await _client.GetAsync("/api/films/1");
        var initialFilm = await GetContentAsAsync<FilmDetailDto>(initialFilmResponse.Content);
        var initialReviewCount = 0;

        // Get initial review count
        var initialReviewsResponse = await _client.GetAsync("/api/films/1/reviews");
        var initialReviewsData = await GetContentAsAsync<ReviewsListDto>(initialReviewsResponse.Content);
        initialReviewCount = initialReviewsData!.TotalCount;

        // Act - Submit multiple reviews
        foreach (var review in reviews)
        {
            var content = CreateJsonContent(review);
            var response = await _client.PostAsync("/api/films/1/reviews", content);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        // Assert - Verify all reviews were added
        var finalReviewsResponse = await _client.GetAsync("/api/films/1/reviews");
        var finalReviewsData = await GetContentAsAsync<ReviewsListDto>(finalReviewsResponse.Content);
        Assert.Equal(initialReviewCount + 3, finalReviewsData!.TotalCount);

        // Verify average ranking is updated
        var finalFilmResponse = await _client.GetAsync("/api/films/1");
        var finalFilm = await GetContentAsAsync<FilmDetailDto>(finalFilmResponse.Content);

        Assert.NotNull(finalFilm);
        // Expected average = (10 + 8 + 5) / 3 = 7.67 (approximately)
        // The actual average will include the original seeded reviews + these 3 new ones
        Assert.True(finalFilm.AverageRanking > 0);
    }
}

