using Application.Features.Projects.DTOs;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.Projects.Queries;

public record GetProjectByIdQuery(int Id) : IRequest<ProjectDto?>;

public class GetProjectByIdQueryHandler : IRequestHandler<GetProjectByIdQuery, ProjectDto?>
{
    private readonly IProjectRepository _projectRepository;

    public GetProjectByIdQueryHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<ProjectDto?> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdAsync(request.Id);

        if (project == null)
            return null;

        return new ProjectDto
        {
            Id = project.Id,
            Title = project.Title,
            Description = project.Description,
            DetailedDescription = project.DetailedDescription,
            Technologies = project.Technologies,
            ProjectUrl = project.ProjectUrl,
            GitHubUrl = project.GitHubUrl,
            ImageUrl = project.ImageUrl,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            IsActive = project.IsActive,
            DisplayOrder = project.DisplayOrder,
            CreatedAt = project.CreatedAt,
            UpdatedAt = project.UpdatedAt
        };
    }
}
