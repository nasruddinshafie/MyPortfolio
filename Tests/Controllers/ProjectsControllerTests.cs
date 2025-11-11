using Microsoft.AspNetCore.Mvc;
using Moq;
using MediatR;
using WebApi.Controllers;
using Application.Features.Projects.DTOs;
using Application.Features.Projects.Queries;
using Application.Features.Projects.Commands;

namespace Tests.Controllers;

public class ProjectsControllerTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly ProjectsController _controller;

    public ProjectsControllerTests()
    {
        _mockMediator = new Mock<IMediator>();
        _controller = new ProjectsController(_mockMediator.Object);
    }

    [Fact]
    public async Task GetAllProjects_Should_Return_OkResult_With_Projects()
    {
        // Arrange
        var projects = new List<ProjectDto>
        {
            new ProjectDto { Id = 1, Title = "Project 1", Description = "Desc 1", Technologies = new List<string> { "C#" }, IsActive = true, DisplayOrder = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new ProjectDto { Id = 2, Title = "Project 2", Description = "Desc 2", Technologies = new List<string> { "React" }, IsActive = true, DisplayOrder = 2, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };

        _mockMediator.Setup(x => x.Send(It.IsAny<GetAllProjectsQuery>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(projects);

        // Act
        var result = await _controller.GetAllProjects();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedProjects = Assert.IsAssignableFrom<IEnumerable<ProjectDto>>(okResult.Value);
        Assert.Equal(2, returnedProjects.Count());

        _mockMediator.Verify(x => x.Send(It.IsAny<GetAllProjectsQuery>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllProjects_Should_Return_Empty_List_When_No_Projects()
    {
        // Arrange
        _mockMediator.Setup(x => x.Send(It.IsAny<GetAllProjectsQuery>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new List<ProjectDto>());

        // Act
        var result = await _controller.GetAllProjects();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedProjects = Assert.IsAssignableFrom<IEnumerable<ProjectDto>>(okResult.Value);
        Assert.Empty(returnedProjects);
    }

    [Fact]
    public async Task GetProjectById_When_Project_Exists_Should_Return_OkResult()
    {
        // Arrange
        var projectDto = new ProjectDto
        {
            Id = 1,
            Title = "Test Project",
            Description = "Test description",
            Technologies = new List<string> { "C#", ".NET" },
            ProjectUrl = "https://test.com",
            IsActive = true,
            DisplayOrder = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockMediator.Setup(x => x.Send(It.Is<GetProjectByIdQuery>(q => q.Id == 1), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(projectDto);

        // Act
        var result = await _controller.GetProjectById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedProject = Assert.IsType<ProjectDto>(okResult.Value);
        Assert.Equal(1, returnedProject.Id);
        Assert.Equal("Test Project", returnedProject.Title);

        _mockMediator.Verify(x => x.Send(It.Is<GetProjectByIdQuery>(q => q.Id == 1), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetProjectById_When_Project_Not_Found_Should_Return_NotFound()
    {
        // Arrange
        _mockMediator.Setup(x => x.Send(It.Is<GetProjectByIdQuery>(q => q.Id == 999), It.IsAny<CancellationToken>()))
                    .ReturnsAsync((ProjectDto?)null);

        // Act
        var result = await _controller.GetProjectById(999);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);

        _mockMediator.Verify(x => x.Send(It.Is<GetProjectByIdQuery>(q => q.Id == 999), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateProject_With_Valid_Data_Should_Return_CreatedResult()
    {
        // Arrange
        var createProjectDto = new CreateProjectDto
        {
            Title = "New Project",
            Description = "New description",
            Technologies = new List<string> { "Python", "Django" },
            ProjectUrl = "https://newproject.com",
            IsActive = true,
            DisplayOrder = 1
        };

        var createdProjectDto = new ProjectDto
        {
            Id = 1,
            Title = createProjectDto.Title,
            Description = createProjectDto.Description,
            Technologies = createProjectDto.Technologies,
            ProjectUrl = createProjectDto.ProjectUrl,
            IsActive = createProjectDto.IsActive,
            DisplayOrder = createProjectDto.DisplayOrder,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockMediator.Setup(x => x.Send(It.IsAny<CreateProjectCommand>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(createdProjectDto);

        // Act
        var result = await _controller.CreateProject(createProjectDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedProject = Assert.IsType<ProjectDto>(createdResult.Value);

        Assert.Equal(1, returnedProject.Id);
        Assert.Equal("New Project", returnedProject.Title);
        Assert.Equal(nameof(_controller.GetProjectById), createdResult.ActionName);
        Assert.Equal(1, createdResult.RouteValues?["id"]);

        _mockMediator.Verify(x => x.Send(It.Is<CreateProjectCommand>(c =>
            c.ProjectDto.Title == "New Project" &&
            c.ProjectDto.Description == "New description"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateProject_With_Minimal_Data_Should_Succeed()
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

        var createdProjectDto = new ProjectDto
        {
            Id = 1,
            Title = createProjectDto.Title,
            Description = createProjectDto.Description,
            Technologies = new List<string>(),
            IsActive = true,
            DisplayOrder = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockMediator.Setup(x => x.Send(It.IsAny<CreateProjectCommand>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(createdProjectDto);

        // Act
        var result = await _controller.CreateProject(createProjectDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedProject = Assert.IsType<ProjectDto>(createdResult.Value);
        Assert.Equal("Minimal Project", returnedProject.Title);
        Assert.Empty(returnedProject.Technologies);
    }

    [Fact]
    public async Task UpdateProject_With_Valid_Data_Should_Return_OkResult()
    {
        // Arrange
        var projectId = 1;
        var updateProjectDto = new UpdateProjectDto
        {
            Title = "Updated Project",
            Description = "Updated description",
            Technologies = new List<string> { "C#", "Azure" },
            IsActive = false,
            DisplayOrder = 5
        };

        var updatedProjectDto = new ProjectDto
        {
            Id = projectId,
            Title = updateProjectDto.Title,
            Description = updateProjectDto.Description,
            Technologies = updateProjectDto.Technologies,
            IsActive = updateProjectDto.IsActive,
            DisplayOrder = updateProjectDto.DisplayOrder,
            CreatedAt = DateTime.UtcNow.AddDays(-30),
            UpdatedAt = DateTime.UtcNow
        };

        _mockMediator.Setup(x => x.Send(It.IsAny<UpdateProjectCommand>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(updatedProjectDto);

        // Act
        var result = await _controller.UpdateProject(projectId, updateProjectDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedProject = Assert.IsType<ProjectDto>(okResult.Value);

        Assert.Equal(projectId, returnedProject.Id);
        Assert.Equal("Updated Project", returnedProject.Title);
        Assert.False(returnedProject.IsActive);
        Assert.Equal(5, returnedProject.DisplayOrder);

        _mockMediator.Verify(x => x.Send(It.Is<UpdateProjectCommand>(c =>
            c.ProjectDto.Id == projectId &&
            c.ProjectDto.Title == "Updated Project"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateProject_When_Project_Not_Found_Should_Return_NotFound()
    {
        // Arrange
        var projectId = 999;
        var updateProjectDto = new UpdateProjectDto
        {
            Title = "Non-existent Project",
            Description = "Description",
            Technologies = new List<string>(),
            IsActive = true,
            DisplayOrder = 1
        };

        _mockMediator.Setup(x => x.Send(It.IsAny<UpdateProjectCommand>(), It.IsAny<CancellationToken>()))
                    .ThrowsAsync(new ArgumentException($"Project with ID {projectId} not found"));

        // Act
        var result = await _controller.UpdateProject(projectId, updateProjectDto);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);

        _mockMediator.Verify(x => x.Send(It.Is<UpdateProjectCommand>(c =>
            c.ProjectDto.Id == projectId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateProject_Should_Set_Id_From_Route_Parameter()
    {
        // Arrange
        var projectId = 42;
        var updateProjectDto = new UpdateProjectDto
        {
            Id = 0, // Should be overridden
            Title = "Test Project",
            Description = "Test description",
            Technologies = new List<string>(),
            IsActive = true,
            DisplayOrder = 1
        };

        var updatedProjectDto = new ProjectDto
        {
            Id = projectId,
            Title = updateProjectDto.Title,
            Description = updateProjectDto.Description,
            Technologies = new List<string>(),
            IsActive = true,
            DisplayOrder = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockMediator.Setup(x => x.Send(It.IsAny<UpdateProjectCommand>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(updatedProjectDto);

        // Act
        await _controller.UpdateProject(projectId, updateProjectDto);

        // Assert
        Assert.Equal(projectId, updateProjectDto.Id);

        _mockMediator.Verify(x => x.Send(It.Is<UpdateProjectCommand>(c =>
            c.ProjectDto.Id == projectId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteProject_Should_Return_NoContent()
    {
        // Arrange
        var projectId = 1;

        _mockMediator.Setup(x => x.Send(It.Is<DeleteProjectCommand>(c => c.Id == projectId), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteProject(projectId);

        // Assert
        Assert.IsType<NoContentResult>(result);

        _mockMediator.Verify(x => x.Send(It.Is<DeleteProjectCommand>(c => c.Id == projectId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteProject_With_Different_Ids_Should_Call_Mediator_With_Correct_Id()
    {
        // Arrange
        var projectIds = new[] { 1, 5, 100, 999 };

        foreach (var id in projectIds)
        {
            _mockMediator.Setup(x => x.Send(It.Is<DeleteProjectCommand>(c => c.Id == id), It.IsAny<CancellationToken>()))
                        .Returns(Task.CompletedTask);

            // Act
            await _controller.DeleteProject(id);

            // Assert
            _mockMediator.Verify(x => x.Send(It.Is<DeleteProjectCommand>(c => c.Id == id), It.IsAny<CancellationToken>()), Times.Once);
        }
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    public async Task GetProjectById_With_Various_Ids_Should_Call_Mediator_Correctly(int projectId)
    {
        // Arrange
        var projectDto = new ProjectDto
        {
            Id = projectId,
            Title = $"Project {projectId}",
            Description = "Description",
            Technologies = new List<string>(),
            IsActive = true,
            DisplayOrder = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockMediator.Setup(x => x.Send(It.Is<GetProjectByIdQuery>(q => q.Id == projectId), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(projectDto);

        // Act
        var result = await _controller.GetProjectById(projectId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedProject = Assert.IsType<ProjectDto>(okResult.Value);
        Assert.Equal(projectId, returnedProject.Id);
    }
}
