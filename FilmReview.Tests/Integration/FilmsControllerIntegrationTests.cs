using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using FileReview.Core.Dtos;
using Xunit;

namespace FilmReview.Tests.Integration;

public class FilmsControllerIntegrationTests : IClassFixture<FilmReviewWebApplicationFactory>
{
    private readonly FilmReviewWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public FilmsControllerIntegrationTests(FilmReviewWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task<T?> GetContentAsAsync<T>(HttpContent content)
    {
        var jsonString = await content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(jsonString, JsonOptions);
    }

    [Fact]
    public async Task GetFilm_WithValidFilmId_ReturnsOkWithFilmDetails()
    {
        // Act
        var response = await _client.GetAsync("/api/films/1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await GetContentAsAsync<FilmDetailDto>(response.Content);
        Assert.NotNull(content);
        Assert.Equal(1, content.FilmId);
        Assert.Equal("Inception", content.Title);
        Assert.Equal(8.8m, content.AverageRanking);
    }

    [Fact]
    public async Task GetFilm_WithValidFilmId_IncludesCastMembers()
    {
        // Act
        var response = await _client.GetAsync("/api/films/1");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await GetContentAsAsync<FilmDetailDto>(response.Content);
        Assert.NotEmpty(content!.CastMembers);
        Assert.Single(content.CastMembers);
        Assert.Equal("Leonardo DiCaprio", content.CastMembers[0].Name);
    }

    [Fact]
    public async Task GetFilm_WithInvalidFilmId_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/films/999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetFilm_WithDifferentValidFilmIds_ReturnsCorrectFilms()
    {
        // Act
        var response1 = await _client.GetAsync("/api/films/1");
        var response2 = await _client.GetAsync("/api/films/2");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response1.StatusCode);
        Assert.Equal(HttpStatusCode.OK, response2.StatusCode);

        var content1 = await GetContentAsAsync<FilmDetailDto>(response1.Content);
        var content2 = await GetContentAsAsync<FilmDetailDto>(response2.Content);

        Assert.Equal("Inception", content1!.Title);
        Assert.Equal("The Matrix", content2!.Title);
    }

    [Fact]
    public async Task GetFilm_ResponseIncludesAllRequiredProperties()
    {
        // Act
        var response = await _client.GetAsync("/api/films/1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await GetContentAsAsync<FilmDetailDto>(response.Content);

        Assert.Equal(1, content!.FilmId);
        Assert.NotNull(content.Title);
        Assert.NotNull(content.PosterUrl);
        Assert.NotNull(content.PlotSummary);
        Assert.True(content.AverageRanking >= 0);
    }

    [Fact]
    public async Task GetFilm_WithZeroFilmId_ReturnsBadRequestOrNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/films/0");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
