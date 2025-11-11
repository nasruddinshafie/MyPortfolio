using Moq;
using Application.Features.Projects.Commands;
using Application.Features.Projects.DTOs;
using Application.Common.Interfaces;
using Domain.Entities;

namespace Tests.Features.Projects;

public class UpdateProjectCommandHandlerTests
{
    private readonly Mock<IProjectRepository> _mockRepository;
    private readonly UpdateProjectCommandHandler _handler;

    public UpdateProjectCommandHandlerTests()
    {
        _mockRepository = new Mock<IProjectRepository>();
        _handler = new UpdateProjectCommandHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_Should_Update_Project_Successfully()
    {
        // Arrange
        var existingProject = new Project
        {
            Id = 1,
            Title = "Original Title",
            Description = "Original description",
            Technologies = new List<string> { "C#" },
            IsActive = true,
            DisplayOrder = 1,
            CreatedAt = DateTime.UtcNow.AddDays(-30),
            UpdatedAt = DateTime.UtcNow.AddDays(-30)
        };

        var updateProjectDto = new UpdateProjectDto
        {
            Id = 1,
            Title = "Updated Title",
            Description = "Updated description",
            DetailedDescription = "New detailed description",
            Technologies = new List<string> { "C#", "Azure" },
            ProjectUrl = "https://updated.com",
            GitHubUrl = "https://github.com/updated",
            IsActive = false,
            DisplayOrder = 2
        };

        var command = new UpdateProjectCommand(updateProjectDto);

        _mockRepository.Setup(x => x.GetByIdAsync(1))
                      .ReturnsAsync(existingProject);
        _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<Project>()))
                      .ReturnsAsync((Project project) => project);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Updated Title", result.Title);
        Assert.Equal("Updated description", result.Description);
        Assert.Equal("New detailed description", result.DetailedDescription);
        Assert.Equal(2, result.Technologies.Count);
        Assert.Contains("Azure", result.Technologies);
        Assert.Equal("https://updated.com", result.ProjectUrl);
        Assert.False(result.IsActive);
        Assert.Equal(2, result.DisplayOrder);

        _mockRepository.Verify(x => x.GetByIdAsync(1), Times.Once);
        _mockRepository.Verify(x => x.UpdateAsync(It.IsAny<Project>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_When_Project_Not_Found()
    {
        // Arrange
        var updateProjectDto = new UpdateProjectDto
        {
            Id = 999,
            Title = "Non-existent Project",
            Description = "Description",
            Technologies = new List<string>(),
            IsActive = true,
            DisplayOrder = 1
        };

        var command = new UpdateProjectCommand(updateProjectDto);

        _mockRepository.Setup(x => x.GetByIdAsync(999))
                      .ReturnsAsync((Project?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _handler.Handle(command, CancellationToken.None));

        Assert.Contains("not found", exception.Message);
        _mockRepository.Verify(x => x.GetByIdAsync(999), Times.Once);
        _mockRepository.Verify(x => x.UpdateAsync(It.IsAny<Project>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Update_Timestamp()
    {
        // Arrange
        var existingProject = new Project
        {
            Id = 1,
            Title = "Original Title",
            Description = "Original description",
            Technologies = new List<string>(),
            IsActive = true,
            DisplayOrder = 1,
            CreatedAt = DateTime.UtcNow.AddDays(-30),
            UpdatedAt = DateTime.UtcNow.AddDays(-30)
        };

        var updateProjectDto = new UpdateProjectDto
        {
            Id = 1,
            Title = "Updated Title",
            Description = "Updated description",
            Technologies = new List<string>(),
            IsActive = true,
            DisplayOrder = 1
        };

        var command = new UpdateProjectCommand(updateProjectDto);
        Project? capturedProject = null;

        _mockRepository.Setup(x => x.GetByIdAsync(1))
                      .ReturnsAsync(existingProject);
        _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<Project>()))
                      .Callback<Project>(project => capturedProject = project)
                      .ReturnsAsync((Project project) => project);

        var beforeUpdate = DateTime.UtcNow;

        // Act
        await _handler.Handle(command, CancellationToken.None);
        var afterUpdate = DateTime.UtcNow;

        // Assert
        Assert.NotNull(capturedProject);
        Assert.True(capturedProject.UpdatedAt >= beforeUpdate && capturedProject.UpdatedAt <= afterUpdate);
        Assert.Equal(existingProject.CreatedAt, capturedProject.CreatedAt); // CreatedAt should not change
    }

    [Fact]
    public async Task Handle_Should_Update_All_Fields()
    {
        // Arrange
        var existingProject = new Project
        {
            Id = 1,
            Title = "Old",
            Description = "Old",
            DetailedDescription = "Old",
            Technologies = new List<string> { "Old" },
            ProjectUrl = "https://old.com",
            GitHubUrl = "https://github.com/old",
            ImageUrl = "/old.png",
            StartDate = DateTime.Parse("2024-01-01"),
            EndDate = DateTime.Parse("2024-12-31"),
            IsActive = true,
            DisplayOrder = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var updateProjectDto = new UpdateProjectDto
        {
            Id = 1,
            Title = "New",
            Description = "New",
            DetailedDescription = "New",
            Technologies = new List<string> { "New" },
            ProjectUrl = "https://new.com",
            GitHubUrl = "https://github.com/new",
            ImageUrl = "/new.png",
            StartDate = DateTime.Parse("2025-01-01"),
            EndDate = null,
            IsActive = false,
            DisplayOrder = 5
        };

        var command = new UpdateProjectCommand(updateProjectDto);

        _mockRepository.Setup(x => x.GetByIdAsync(1))
                      .ReturnsAsync(existingProject);
        _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<Project>()))
                      .ReturnsAsync((Project project) => project);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("New", result.Title);
        Assert.Equal("New", result.Description);
        Assert.Equal("New", result.DetailedDescription);
        Assert.Single(result.Technologies);
        Assert.Contains("New", result.Technologies);
        Assert.Equal("https://new.com", result.ProjectUrl);
        Assert.Equal("https://github.com/new", result.GitHubUrl);
        Assert.Equal("/new.png", result.ImageUrl);
        Assert.Equal(DateTime.Parse("2025-01-01"), result.StartDate);
        Assert.Null(result.EndDate);
        Assert.False(result.IsActive);
        Assert.Equal(5, result.DisplayOrder);
    }
}
