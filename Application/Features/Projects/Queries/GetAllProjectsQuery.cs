using Application.Features.Projects.DTOs;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.Projects.Queries;

public record GetAllProjectsQuery : IRequest<IEnumerable<ProjectDto>>;

public class GetAllProjectsQueryHandler : IRequestHandler<GetAllProjectsQuery, IEnumerable<ProjectDto>>
{
    private readonly IProjectRepository _projectRepository;

    public GetAllProjectsQueryHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<IEnumerable<ProjectDto>> Handle(GetAllProjectsQuery request, CancellationToken cancellationToken)
    {
        var projects = await _projectRepository.GetAllAsync();

        return projects.Select(p => new ProjectDto
        {
            Id = p.Id,
            Title = p.Title,
            Description = p.Description,
            DetailedDescription = p.DetailedDescription,
            Technologies = p.Technologies,
            ProjectUrl = p.ProjectUrl,
            GitHubUrl = p.GitHubUrl,
            ImageUrl = p.ImageUrl,
            StartDate = p.StartDate,
            EndDate = p.EndDate,
            IsActive = p.IsActive,
            DisplayOrder = p.DisplayOrder,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        });
    }
}
