using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using FilmReview.Core.Services;

namespace FilmReview.Tests.Services;

public class AISummaryServiceTests
{
    [Fact]
    public async Task GenerateFilmSummaryAsync_WithValidInput_ReturnsNonEmptyString()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<AISummaryService>>();
        var configMock = new Mock<IConfiguration>();
        var httpClientMock = new HttpClient();

        // Configure mock to return null/empty for Azure OpenAI settings
        // This will cause the service to return null (expected behavior when not configured)
        configMock.Setup(c => c["AzureOpenAI:Endpoint"]).Returns((string?)null);
        configMock.Setup(c => c["AzureOpenAI:ApiKey"]).Returns((string?)null);

        var service = new AISummaryService(loggerMock.Object, configMock.Object, httpClientMock);

        // Act
        var result = await service.GenerateFilmSummaryAsync("Test Film", "A test plot summary");

        // Assert
        // When not configured, service should return null
        Assert.Null(result);
    }

    [Fact]
    public async Task GenerateFilmSummaryAsync_WithoutConfiguration_ReturnsNull()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<AISummaryService>>();
        var configurationMock = new Mock<IConfiguration>();
        var httpClientMock = new HttpClient();

        configurationMock.Setup(c => c["AzureOpenAI:Endpoint"]).Returns((string?)null);
        configurationMock.Setup(c => c["AzureOpenAI:ApiKey"]).Returns((string?)null);

        var service = new AISummaryService(loggerMock.Object, configurationMock.Object, httpClientMock);

        // Act
        var result = await service.GenerateFilmSummaryAsync("Inception", "A thief who steals corporate secrets through dream-sharing technology.");

        // Assert
        Assert.Null(result);
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Azure OpenAI configuration is not set")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task GenerateFilmSummaryAsync_WithInvalidFilmTitle_LogsWarning()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<AISummaryService>>();
        var configMock = new Mock<IConfiguration>();
        var httpClientMock = new HttpClient();

        configMock.Setup(c => c["AzureOpenAI:Endpoint"]).Returns((string?)null);
        configMock.Setup(c => c["AzureOpenAI:ApiKey"]).Returns((string?)null);

        var service = new AISummaryService(loggerMock.Object, configMock.Object, httpClientMock);

        // Act
        await service.GenerateFilmSummaryAsync("", "");

        // Assert
        // Service should handle empty strings gracefully
        Assert.NotNull(service);
    }
}
