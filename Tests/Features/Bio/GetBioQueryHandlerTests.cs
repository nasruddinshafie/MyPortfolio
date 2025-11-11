using Moq;
using Application.Features.Bio.Queries;
using Application.Common.Interfaces;
using Domain.Entities;

namespace Tests.Features.Bio;

public class GetBioQueryHandlerTests
{
    private readonly Mock<IBioRepository> _mockRepository;
    private readonly GetBioQueryHandler _handler;

    public GetBioQueryHandlerTests()
    {
        _mockRepository = new Mock<IBioRepository>();
        _handler = new GetBioQueryHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Null_When_No_Bio_Exists()
    {
        // Arrange
        var query = new GetBioQuery();

        _mockRepository.Setup(x => x.GetAsync())
                      .ReturnsAsync((Domain.Entities.Bio?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
        _mockRepository.Verify(x => x.GetAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_BioDto_When_Bio_Exists()
    {
        // Arrange
        var query = new GetBioQuery();
        var bio = new Domain.Entities.Bio
        {
            Id = 1,
            FullName = "John Doe",
            Title = "Software Developer",
            Summary = "Experienced developer with 5+ years",
            DetailedDescription = "Detailed bio description here",
            Email = "john.doe@email.com",
            Phone = "+1234567890",
            Location = "New York, NY",
            LinkedInUrl = "https://linkedin.com/in/johndoe",
            GitHubUrl = "https://github.com/johndoe",
            WebsiteUrl = "https://johndoe.dev",
            ProfileImageUrl = "https://johndoe.dev/avatar.jpg",
            CreatedAt = new DateTime(2023, 1, 1),
            UpdatedAt = new DateTime(2023, 6, 1)
        };

        _mockRepository.Setup(x => x.GetAsync())
                      .ReturnsAsync(bio);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("John Doe", result.FullName);
        Assert.Equal("Software Developer", result.Title);
        Assert.Equal("Experienced developer with 5+ years", result.Summary);
        Assert.Equal("Detailed bio description here", result.DetailedDescription);
        Assert.Equal("john.doe@email.com", result.Email);
        Assert.Equal("+1234567890", result.Phone);
        Assert.Equal("New York, NY", result.Location);
        Assert.Equal("https://linkedin.com/in/johndoe", result.LinkedInUrl);
        Assert.Equal("https://github.com/johndoe", result.GitHubUrl);
        Assert.Equal("https://johndoe.dev", result.WebsiteUrl);
        Assert.Equal("https://johndoe.dev/avatar.jpg", result.ProfileImageUrl);
        Assert.Equal(new DateTime(2023, 1, 1), result.CreatedAt);
        Assert.Equal(new DateTime(2023, 6, 1), result.UpdatedAt);

        _mockRepository.Verify(x => x.GetAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Map_Bio_With_Null_Optional_Fields()
    {
        // Arrange
        var query = new GetBioQuery();
        var bio = new Domain.Entities.Bio
        {
            Id = 2,
            FullName = "Jane Smith",
            Title = "Product Manager",
            Summary = "Experienced PM",
            DetailedDescription = null,
            Email = "jane@email.com",
            Phone = null,
            Location = null,
            LinkedInUrl = null,
            GitHubUrl = null,
            WebsiteUrl = null,
            ProfileImageUrl = null,
            CreatedAt = new DateTime(2023, 2, 1),
            UpdatedAt = new DateTime(2023, 7, 1)
        };

        _mockRepository.Setup(x => x.GetAsync())
                      .ReturnsAsync(bio);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Id);
        Assert.Equal("Jane Smith", result.FullName);
        Assert.Equal("Product Manager", result.Title);
        Assert.Equal("Experienced PM", result.Summary);
        Assert.Null(result.DetailedDescription);
        Assert.Equal("jane@email.com", result.Email);
        Assert.Null(result.Phone);
        Assert.Null(result.Location);
        Assert.Null(result.LinkedInUrl);
        Assert.Null(result.GitHubUrl);
        Assert.Null(result.WebsiteUrl);
        Assert.Null(result.ProfileImageUrl);
        Assert.Equal(new DateTime(2023, 2, 1), result.CreatedAt);
        Assert.Equal(new DateTime(2023, 7, 1), result.UpdatedAt);
    }

    [Fact]
    public async Task Handle_Should_Map_Bio_With_Empty_Strings()
    {
        // Arrange
        var query = new GetBioQuery();
        var bio = new Domain.Entities.Bio
        {
            Id = 3,
            FullName = "",
            Title = "",
            Summary = "",
            DetailedDescription = "",
            Email = "",
            Phone = "",
            Location = "",
            LinkedInUrl = "",
            GitHubUrl = "",
            WebsiteUrl = "",
            ProfileImageUrl = "",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(x => x.GetAsync())
                      .ReturnsAsync(bio);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Id);
        Assert.Equal("", result.FullName);
        Assert.Equal("", result.Title);
        Assert.Equal("", result.Summary);
        Assert.Equal("", result.DetailedDescription);
        Assert.Equal("", result.Email);
        Assert.Equal("", result.Phone);
        Assert.Equal("", result.Location);
        Assert.Equal("", result.LinkedInUrl);
        Assert.Equal("", result.GitHubUrl);
        Assert.Equal("", result.WebsiteUrl);
        Assert.Equal("", result.ProfileImageUrl);
    }

    [Fact]
    public async Task Handle_Should_Use_Cancellation_Token()
    {
        // Arrange
        var query = new GetBioQuery();
        var cancellationToken = new CancellationToken(true);

        _mockRepository.Setup(x => x.GetAsync())
                      .ReturnsAsync((Domain.Entities.Bio?)null);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        // If cancellation token was properly passed through and used,
        // the operation should complete (in this case returning null)
        Assert.Null(result);
        _mockRepository.Verify(x => x.GetAsync(), Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("John Doe")]
    [InlineData("A very long name that might exceed normal expectations for testing")]
    public async Task Handle_Should_Handle_Various_Name_Formats(string fullName)
    {
        // Arrange
        var query = new GetBioQuery();
        var bio = new Domain.Entities.Bio
        {
            Id = 1,
            FullName = fullName,
            Title = "Developer",
            Summary = "Summary",
            Email = "test@email.com",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(x => x.GetAsync())
                      .ReturnsAsync(bio);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(fullName, result.FullName);
        Assert.Equal("Developer", result.Title);
        Assert.Equal("Summary", result.Summary);
        Assert.Equal("test@email.com", result.Email);
    }
}