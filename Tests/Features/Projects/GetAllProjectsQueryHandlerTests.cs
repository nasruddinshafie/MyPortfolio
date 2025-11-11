using Moq;
using Application.Features.Projects.Queries;
using Application.Common.Interfaces;
using Domain.Entities;

namespace Tests.Features.Projects;

public class GetAllProjectsQueryHandlerTests
{
    private readonly Mock<IProjectRepository> _mockRepository;
    private readonly GetAllProjectsQueryHandler _handler;

    public GetAllProjectsQueryHandlerTests()
    {
        _mockRepository = new Mock<IProjectRepository>();
        _handler = new GetAllProjectsQueryHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_All_Projects()
    {
        // Arrange
        var projects = new List<Project>
        {
            new Project
            {
                Id = 1,
                Title = "Project One",
                Description = "Description one",
                Technologies = new List<string> { "C#", ".NET" },
                IsActive = true,
                DisplayOrder = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Project
            {
                Id = 2,
                Title = "Project Two",
                Description = "Description two",
                Technologies = new List<string> { "React", "TypeScript" },
                IsActive = true,
                DisplayOrder = 2,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        _mockRepository.Setup(x => x.GetAllAsync())
                      .ReturnsAsync(projects);

        var query = new GetAllProjectsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        var projectsList = result.ToList();
        Assert.Equal(2, projectsList.Count);

        Assert.Equal(1, projectsList[0].Id);
        Assert.Equal("Project One", projectsList[0].Title);
        Assert.Equal(2, projectsList[0].Technologies.Count);

        Assert.Equal(2, projectsList[1].Id);
        Assert.Equal("Project Two", projectsList[1].Title);

        _mockRepository.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_Empty_List_When_No_Projects()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetAllAsync())
                      .ReturnsAsync(new List<Project>());

        var query = new GetAllProjectsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);

        _mockRepository.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Map_All_Properties_Correctly()
    {
        // Arrange
        var project = new Project
        {
            Id = 1,
            Title = "Full Project",
            Description = "Short description",
            DetailedDescription = "Long detailed description",
            Technologies = new List<string> { "C#", "Azure", "React" },
            ProjectUrl = "https://project.com",
            GitHubUrl = "https://github.com/project",
            ImageUrl = "/images/project.png",
            StartDate = DateTime.Parse("2025-01-01"),
            EndDate = DateTime.Parse("2025-12-31"),
            IsActive = true,
            DisplayOrder = 3,
            CreatedAt = DateTime.Parse("2025-01-01"),
            UpdatedAt = DateTime.Parse("2025-02-01")
        };

        _mockRepository.Setup(x => x.GetAllAsync())
                      .ReturnsAsync(new List<Project> { project });

        var query = new GetAllProjectsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var projectDto = result.First();
        Assert.Equal(1, projectDto.Id);
        Assert.Equal("Full Project", projectDto.Title);
        Assert.Equal("Short description", projectDto.Description);
        Assert.Equal("Long detailed description", projectDto.DetailedDescription);
        Assert.Equal(3, projectDto.Technologies.Count);
        Assert.Contains("C#", projectDto.Technologies);
        Assert.Equal("https://project.com", projectDto.ProjectUrl);
        Assert.Equal("https://github.com/project", projectDto.GitHubUrl);
        Assert.Equal("/images/project.png", projectDto.ImageUrl);
        Assert.Equal(DateTime.Parse("2025-01-01"), projectDto.StartDate);
        Assert.Equal(DateTime.Parse("2025-12-31"), projectDto.EndDate);
        Assert.True(projectDto.IsActive);
        Assert.Equal(3, projectDto.DisplayOrder);
        Assert.Equal(DateTime.Parse("2025-01-01"), projectDto.CreatedAt);
        Assert.Equal(DateTime.Parse("2025-02-01"), projectDto.UpdatedAt);
    }

    [Fact]
    public async Task Handle_Should_Include_Inactive_Projects()
    {
        // Arrange
        var projects = new List<Project>
        {
            new Project { Id = 1, Title = "Active", Description = "Desc", Technologies = new List<string>(), IsActive = true, DisplayOrder = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Project { Id = 2, Title = "Inactive", Description = "Desc", Technologies = new List<string>(), IsActive = false, DisplayOrder = 2, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };

        _mockRepository.Setup(x => x.GetAllAsync())
                      .ReturnsAsync(projects);

        var query = new GetAllProjectsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, p => p.IsActive);
        Assert.Contains(result, p => !p.IsActive);
    }
}
