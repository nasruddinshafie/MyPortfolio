using Application.Features.Projects.DTOs;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.Projects.Commands;

public record CreateProjectCommand(CreateProjectDto ProjectDto) : IRequest<ProjectDto>;

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, ProjectDto>
{
    private readonly IProjectRepository _projectRepository;

    public CreateProjectCommandHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<ProjectDto> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = new Domain.Entities.Project
        {
            Title = request.ProjectDto.Title,
            Description = request.ProjectDto.Description,
            DetailedDescription = request.ProjectDto.DetailedDescription,
            Technologies = request.ProjectDto.Technologies,
            ProjectUrl = request.ProjectDto.ProjectUrl,
            GitHubUrl = request.ProjectDto.GitHubUrl,
            ImageUrl = request.ProjectDto.ImageUrl,
            StartDate = request.ProjectDto.StartDate,
            EndDate = request.ProjectDto.EndDate,
            IsActive = request.ProjectDto.IsActive,
            DisplayOrder = request.ProjectDto.DisplayOrder,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdProject = await _projectRepository.CreateAsync(project);

        return new ProjectDto
        {
            Id = createdProject.Id,
            Title = createdProject.Title,
            Description = createdProject.Description,
            DetailedDescription = createdProject.DetailedDescription,
            Technologies = createdProject.Technologies,
            ProjectUrl = createdProject.ProjectUrl,
            GitHubUrl = createdProject.GitHubUrl,
            ImageUrl = createdProject.ImageUrl,
            StartDate = createdProject.StartDate,
            EndDate = createdProject.EndDate,
            IsActive = createdProject.IsActive,
            DisplayOrder = createdProject.DisplayOrder,
            CreatedAt = createdProject.CreatedAt,
            UpdatedAt = createdProject.UpdatedAt
        };
    }
}
