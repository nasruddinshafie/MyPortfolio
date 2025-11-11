using Application.Features.Projects.DTOs;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.Projects.Commands;

public record UpdateProjectCommand(UpdateProjectDto ProjectDto) : IRequest<ProjectDto>;

public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, ProjectDto>
{
    private readonly IProjectRepository _projectRepository;

    public UpdateProjectCommandHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<ProjectDto> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var existingProject = await _projectRepository.GetByIdAsync(request.ProjectDto.Id);
        if (existingProject == null)
        {
            throw new ArgumentException($"Project with ID {request.ProjectDto.Id} not found");
        }

        existingProject.Title = request.ProjectDto.Title;
        existingProject.Description = request.ProjectDto.Description;
        existingProject.DetailedDescription = request.ProjectDto.DetailedDescription;
        existingProject.Technologies = request.ProjectDto.Technologies;
        existingProject.ProjectUrl = request.ProjectDto.ProjectUrl;
        existingProject.GitHubUrl = request.ProjectDto.GitHubUrl;
        existingProject.ImageUrl = request.ProjectDto.ImageUrl;
        existingProject.StartDate = request.ProjectDto.StartDate;
        existingProject.EndDate = request.ProjectDto.EndDate;
        existingProject.IsActive = request.ProjectDto.IsActive;
        existingProject.DisplayOrder = request.ProjectDto.DisplayOrder;
        existingProject.UpdatedAt = DateTime.UtcNow;

        var updatedProject = await _projectRepository.UpdateAsync(existingProject);

        return new ProjectDto
        {
            Id = updatedProject.Id,
            Title = updatedProject.Title,
            Description = updatedProject.Description,
            DetailedDescription = updatedProject.DetailedDescription,
            Technologies = updatedProject.Technologies,
            ProjectUrl = updatedProject.ProjectUrl,
            GitHubUrl = updatedProject.GitHubUrl,
            ImageUrl = updatedProject.ImageUrl,
            StartDate = updatedProject.StartDate,
            EndDate = updatedProject.EndDate,
            IsActive = updatedProject.IsActive,
            DisplayOrder = updatedProject.DisplayOrder,
            CreatedAt = updatedProject.CreatedAt,
            UpdatedAt = updatedProject.UpdatedAt
        };
    }
}
