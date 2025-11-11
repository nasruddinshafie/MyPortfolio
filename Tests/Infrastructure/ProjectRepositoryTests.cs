using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
using Infrastructure.Features.Projects.Repositories;
using Domain.Entities;

namespace Tests.Infrastructure;

public class ProjectRepositoryTests : IDisposable
{
    private readonly PortfolioDbContext _context;
    private readonly ProjectRepository _repository;

    public ProjectRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<PortfolioDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new PortfolioDbContext(options);
        _repository = new ProjectRepository(_context);
    }

    [Fact]
    public async Task GetAllAsync_WhenNoProjectsExist_ShouldReturnEmptyList()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_WhenProjectsExist_ShouldReturnAllProjects()
    {
        // Arrange
        var project1 = new Project
        {
            Title = "First Project",
            Description = "First description",
            Technologies = new List<string> { "C#", ".NET" },
            DisplayOrder = 2,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var project2 = new Project
        {
            Title = "Second Project",
            Description = "Second description",
            Technologies = new List<string> { "React", "TypeScript" },
            DisplayOrder = 1,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Projects.AddRange(project1, project2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());

        // Should be ordered by DisplayOrder
        var projects = result.ToList();
        Assert.Equal("Second Project", projects[0].Title);
        Assert.Equal("First Project", projects[1].Title);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProjectExists_ShouldReturnProject()
    {
        // Arrange
        var project = new Project
        {
            Title = "Test Project",
            Description = "Test description",
            Technologies = new List<string> { "C#", "ASP.NET Core" },
            ProjectUrl = "https://example.com",
            GitHubUrl = "https://github.com/example",
            DisplayOrder = 1,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(project.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Project", result.Title);
        Assert.Equal("Test description", result.Description);
        Assert.Equal(2, result.Technologies.Count);
        Assert.Contains("C#", result.Technologies);
        Assert.Equal("https://example.com", result.ProjectUrl);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProjectDoesNotExist_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateProjectSuccessfully()
    {
        // Arrange
        var project = new Project
        {
            Title = "New Project",
            Description = "New description",
            DetailedDescription = "Detailed information about the project",
            Technologies = new List<string> { "Python", "Django", "PostgreSQL" },
            ProjectUrl = "https://newproject.com",
            GitHubUrl = "https://github.com/new",
            ImageUrl = "/images/project.png",
            StartDate = DateTime.Parse("2025-01-01"),
            EndDate = null,
            IsActive = true,
            DisplayOrder = 5,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        var result = await _repository.CreateAsync(project);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("New Project", result.Title);
        Assert.Equal("New description", result.Description);
        Assert.Equal(3, result.Technologies.Count);
        Assert.Contains("Python", result.Technologies);
        Assert.Equal(5, result.DisplayOrder);

        var savedProject = await _context.Projects.FindAsync(result.Id);
        Assert.NotNull(savedProject);
        Assert.Equal("New Project", savedProject.Title);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateProjectSuccessfully()
    {
        // Arrange
        var project = new Project
        {
            Title = "Original Title",
            Description = "Original description",
            Technologies = new List<string> { "C#" },
            DisplayOrder = 1,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        // Modify the project
        project.Title = "Updated Title";
        project.Description = "Updated description";
        project.Technologies = new List<string> { "C#", "Azure" };
        project.DisplayOrder = 2;
        project.IsActive = false;
        project.UpdatedAt = DateTime.UtcNow;

        // Act
        var result = await _repository.UpdateAsync(project);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Title", result.Title);
        Assert.Equal("Updated description", result.Description);
        Assert.Equal(2, result.Technologies.Count);
        Assert.Contains("Azure", result.Technologies);
        Assert.Equal(2, result.DisplayOrder);
        Assert.False(result.IsActive);

        var updatedProject = await _context.Projects.FindAsync(result.Id);
        Assert.NotNull(updatedProject);
        Assert.Equal("Updated Title", updatedProject.Title);
    }

    [Fact]
    public async Task DeleteAsync_WhenProjectExists_ShouldDeleteProject()
    {
        // Arrange
        var project = new Project
        {
            Title = "To Delete",
            Description = "Delete description",
            Technologies = new List<string> { "Java" },
            DisplayOrder = 1,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
        var projectId = project.Id;

        // Act
        await _repository.DeleteAsync(projectId);

        // Assert
        var deletedProject = await _context.Projects.FindAsync(projectId);
        Assert.Null(deletedProject);
    }

    [Fact]
    public async Task DeleteAsync_WhenProjectDoesNotExist_ShouldNotThrow()
    {
        // Act & Assert
        await _repository.DeleteAsync(999);

        // Should not throw exception
        Assert.True(true);
    }

    [Fact]
    public async Task CreateAsync_WithMinimalData_ShouldSucceed()
    {
        // Arrange
        var project = new Project
        {
            Title = "Minimal Project",
            Description = "Minimal description",
            Technologies = new List<string>(),
            DisplayOrder = 1,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        var result = await _repository.CreateAsync(project);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("Minimal Project", result.Title);
        Assert.Empty(result.Technologies);
    }

    [Fact]
    public async Task GetAllAsync_ShouldOrderByDisplayOrder()
    {
        // Arrange
        var projects = new[]
        {
            new Project { Title = "Third", Description = "Desc", Technologies = new List<string>(), DisplayOrder = 3, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Project { Title = "First", Description = "Desc", Technologies = new List<string>(), DisplayOrder = 1, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Project { Title = "Second", Description = "Desc", Technologies = new List<string>(), DisplayOrder = 2, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };

        _context.Projects.AddRange(projects);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        var projectsList = result.ToList();
        Assert.Equal("First", projectsList[0].Title);
        Assert.Equal("Second", projectsList[1].Title);
        Assert.Equal("Third", projectsList[2].Title);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
