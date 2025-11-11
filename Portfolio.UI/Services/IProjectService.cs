using Portfolio.UI.DTOs;

namespace Portfolio.UI.Services;

public interface IProjectService
{
    Task<IEnumerable<ProjectDto>> GetAllProjectsAsync();
    Task<ProjectDto?> GetProjectByIdAsync(int id);
    Task<ProjectDto> CreateProjectAsync(CreateProjectDto createProjectDto);
    Task<ProjectDto> UpdateProjectAsync(int id, UpdateProjectDto updateProjectDto);
    Task DeleteProjectAsync(int id);
}
