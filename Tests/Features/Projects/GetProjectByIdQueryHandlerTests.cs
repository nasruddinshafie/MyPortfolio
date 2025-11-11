using Moq;
using Application.Features.Projects.Queries;
using Application.Common.Interfaces;
using Domain.Entities;

namespace Tests.Features.Projects;

public class GetProjectByIdQueryHandlerTests
{
    private readonly Mock<IProjectRepository> _mockRepository;
    private readonly GetProjectByIdQueryHandler _handler;

    public GetProjectByIdQueryHandlerTests()
    {
        _mockRepository = new Mock<IProjectRepository>();
        _handler = new GetProjectByIdQueryHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Project_When_Exists()
    {
        // Arrange
        var project = new Project
        {
            Id = 1,
            Title = "Test Project",
            Description = "Test description",
            DetailedDescription = "Detailed test description",
            Technologies = new List<string> { "C#", ".NET", "Azure" },
            ProjectUrl = "https://test.com",
            GitHubUrl = "https://github.com/test",
            ImageUrl = "/images/test.png",
            StartDate = DateTime.Parse("2025-01-01"),
            EndDate = null,
            IsActive = true,
            DisplayOrder = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(x => x.GetByIdAsync(1))
                      .ReturnsAsync(project);

        var query = new GetProjectByIdQuery(1);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test Project", result.Title);
        Assert.Equal("Test description", result.Description);
        Assert.Equal("Detailed test description", result.DetailedDescription);
        Assert.Equal(3, result.Technologies.Count);
        Assert.Contains("C#", result.Technologies);
        Assert.Equal("https://test.com", result.ProjectUrl);
        Assert.Equal("https://github.com/test", result.GitHubUrl);
        Assert.Equal("/images/test.png", result.ImageUrl);
        Assert.Equal(DateTime.Parse("2025-01-01"), result.StartDate);
        Assert.Null(result.EndDate);
        Assert.True(result.IsActive);
        Assert.Equal(1, result.DisplayOrder);

        _mockRepository.Verify(x => x.GetByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_Null_When_Project_Not_Found()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetByIdAsync(999))
                      .ReturnsAsync((Project?)null);

        var query = new GetProjectByIdQuery(999);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);

        _mockRepository.Verify(x => x.GetByIdAsync(999), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Map_All_Properties_Correctly()
    {
        // Arrange
        var project = new Project
        {
            Id = 42,
            Title = "Complex Project",
            Description = "A complex project with all fields",
            DetailedDescription = "Very detailed information about this complex project",
            Technologies = new List<string> { "Python", "Django", "PostgreSQL", "Docker", "Kubernetes" },
            ProjectUrl = "https://complex.com",
            GitHubUrl = "https://github.com/complex",
            ImageUrl = "/images/complex.jpg",
            StartDate = DateTime.Parse("2024-06-15"),
            EndDate = DateTime.Parse("2025-06-15"),
            IsActive = false,
            DisplayOrder = 10,
            CreatedAt = DateTime.Parse("2024-06-15T10:30:00"),
            UpdatedAt = DateTime.Parse("2025-01-10T14:45:00")
        };

        _mockRepository.Setup(x => x.GetByIdAsync(42))
                      .ReturnsAsync(project);

        var query = new GetProjectByIdQuery(42);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(42, result.Id);
        Assert.Equal("Complex Project", result.Title);
        Assert.Equal("A complex project with all fields", result.Description);
        Assert.Equal("Very detailed information about this complex project", result.DetailedDescription);
        Assert.Equal(5, result.Technologies.Count);
        Assert.Contains("Python", result.Technologies);
        Assert.Contains("Kubernetes", result.Technologies);
        Assert.Equal("https://complex.com", result.ProjectUrl);
        Assert.Equal("https://github.com/complex", result.GitHubUrl);
        Assert.Equal("/images/complex.jpg", result.ImageUrl);
        Assert.Equal(DateTime.Parse("2024-06-15"), result.StartDate);
        Assert.Equal(DateTime.Parse("2025-06-15"), result.EndDate);
        Assert.False(result.IsActive);
        Assert.Equal(10, result.DisplayOrder);
        Assert.Equal(DateTime.Parse("2024-06-15T10:30:00"), result.CreatedAt);
        Assert.Equal(DateTime.Parse("2025-01-10T14:45:00"), result.UpdatedAt);
    }

    [Fact]
    public async Task Handle_Should_Handle_Project_With_Empty_Technologies()
    {
        // Arrange
        var project = new Project
        {
            Id = 1,
            Title = "Simple Project",
            Description = "Simple description",
            Technologies = new List<string>(),
            IsActive = true,
            DisplayOrder = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(x => x.GetByIdAsync(1))
                      .ReturnsAsync(project);

        var query = new GetProjectByIdQuery(1);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Technologies);
    }
}
