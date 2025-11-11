using Moq;
using Application.Features.Projects.Commands;
using Application.Features.Projects.DTOs;
using Application.Common.Interfaces;
using Domain.Entities;

namespace Tests.Features.Projects;

public class CreateProjectCommandHandlerTests
{
    private readonly Mock<IProjectRepository> _mockRepository;
    private readonly CreateProjectCommandHandler _handler;

    public CreateProjectCommandHandlerTests()
    {
        _mockRepository = new Mock<IProjectRepository>();
        _handler = new CreateProjectCommandHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_Should_Create_Project_Successfully()
    {
        // Arrange
        var createProjectDto = new CreateProjectDto
        {
            Title = "My Portfolio Website",
            Description = "A personal portfolio built with .NET",
            DetailedDescription = "Detailed information about the portfolio project",
            Technologies = new List<string> { "C#", ".NET 9", "SQL Server" },
            ProjectUrl = "https://myportfolio.com",
            GitHubUrl = "https://github.com/user/portfolio",
            ImageUrl = "/images/portfolio.png",
            StartDate = DateTime.Parse("2025-01-01"),
            EndDate = null,
            IsActive = true,
            DisplayOrder = 1
        };

        var command = new CreateProjectCommand(createProjectDto);
        var createdProject = new Project
        {
            Id = 1,
            Title = createProjectDto.Title,
            Description = createProjectDto.Description,
            DetailedDescription = createProjectDto.DetailedDescription,
            Technologies = createProjectDto.Technologies,
            ProjectUrl = createProjectDto.ProjectUrl,
            GitHubUrl = createProjectDto.GitHubUrl,
            ImageUrl = createProjectDto.ImageUrl,
            StartDate = createProjectDto.StartDate,
            EndDate = createProjectDto.EndDate,
            IsActive = createProjectDto.IsActive,
            DisplayOrder = createProjectDto.DisplayOrder,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(x => x.CreateAsync(It.IsAny<Project>()))
                      .ReturnsAsync(createdProject);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("My Portfolio Website", result.Title);
        Assert.Equal("A personal portfolio built with .NET", result.Description);
        Assert.Equal(3, result.Technologies.Count);
        Assert.Contains("C#", result.Technologies);
        Assert.Equal("https://myportfolio.com", result.ProjectUrl);
        Assert.Equal("https://github.com/user/portfolio", result.GitHubUrl);
        Assert.True(result.IsActive);
        Assert.Equal(1, result.DisplayOrder);

        _mockRepository.Verify(x => x.CreateAsync(It.Is<Project>(p =>
            p.Title == "My Portfolio Website" &&
            p.Description == "A personal portfolio built with .NET" &&
            p.Technologies.Count == 3)), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Set_Timestamps_When_Creating_Project()
    {
        // Arrange
        var createProjectDto = new CreateProjectDto
        {
            Title = "Test Project",
            Description = "Test description",
            Technologies = new List<string> { "Python" },
            IsActive = true,
            DisplayOrder = 1
        };

        var command = new CreateProjectCommand(createProjectDto);
        Project? capturedProject = null;

        _mockRepository.Setup(x => x.CreateAsync(It.IsAny<Project>()))
                      .Callback<Project>(project => capturedProject = project)
                      .ReturnsAsync((Project project) => project);

        var beforeCreate = DateTime.UtcNow;

        // Act
        await _handler.Handle(command, CancellationToken.None);
        var afterCreate = DateTime.UtcNow;

        // Assert
        Assert.NotNull(capturedProject);
        Assert.True(capturedProject.CreatedAt >= beforeCreate && capturedProject.CreatedAt <= afterCreate);
        Assert.True(capturedProject.UpdatedAt >= beforeCreate && capturedProject.UpdatedAt <= afterCreate);

        // CreatedAt and UpdatedAt should be very close (within 1 second)
        var timeDifference = Math.Abs((capturedProject.CreatedAt - capturedProject.UpdatedAt).TotalSeconds);
        Assert.True(timeDifference < 1, $"CreatedAt and UpdatedAt should be within 1 second, but differ by {timeDifference} seconds");

        _mockRepository.Verify(x => x.CreateAsync(It.IsAny<Project>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Handle_Minimal_Project_Data()
    {
        // Arrange
        var createProjectDto = new CreateProjectDto
        {
            Title = "Minimal Project",
            Description = "Minimal description",
            Technologies = new List<string>(),
            IsActive = true,
            DisplayOrder = 1
        };

        var command = new CreateProjectCommand(createProjectDto);
        var createdProject = new Project
        {
            Id = 1,
            Title = createProjectDto.Title,
            Description = createProjectDto.Description,
            Technologies = createProjectDto.Technologies,
            IsActive = createProjectDto.IsActive,
            DisplayOrder = createProjectDto.DisplayOrder,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(x => x.CreateAsync(It.IsAny<Project>()))
                      .ReturnsAsync(createdProject);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Minimal Project", result.Title);
        Assert.Equal("Minimal description", result.Description);
        Assert.Empty(result.Technologies);
        Assert.Null(result.ProjectUrl);
        Assert.Null(result.GitHubUrl);
        Assert.Null(result.ImageUrl);
    }

    [Fact]
    public async Task Handle_Should_Handle_Multiple_Technologies()
    {
        // Arrange
        var technologies = new List<string> { "React", "Node.js", "MongoDB", "Docker", "AWS" };
        var createProjectDto = new CreateProjectDto
        {
            Title = "Full Stack App",
            Description = "A comprehensive full stack application",
            Technologies = technologies,
            IsActive = true,
            DisplayOrder = 1
        };

        var command = new CreateProjectCommand(createProjectDto);
        _mockRepository.Setup(x => x.CreateAsync(It.IsAny<Project>()))
                      .ReturnsAsync((Project project) => project);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockRepository.Verify(x => x.CreateAsync(It.Is<Project>(p =>
            p.Technologies.Count == 5 &&
            p.Technologies.Contains("React") &&
            p.Technologies.Contains("AWS"))), Times.Once);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Handle_Should_Respect_IsActive_Flag(bool isActive)
    {
        // Arrange
        var createProjectDto = new CreateProjectDto
        {
            Title = "Test Project",
            Description = "Test description",
            Technologies = new List<string> { "C#" },
            IsActive = isActive,
            DisplayOrder = 1
        };

        var command = new CreateProjectCommand(createProjectDto);
        _mockRepository.Setup(x => x.CreateAsync(It.IsAny<Project>()))
                      .ReturnsAsync((Project project) => project);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockRepository.Verify(x => x.CreateAsync(It.Is<Project>(p =>
            p.IsActive == isActive)), Times.Once);
    }
}
