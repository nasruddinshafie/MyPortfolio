using Domain.Entities;

namespace Application.Common.Interfaces;

public interface IProjectRepository
{
    Task<IEnumerable<Project>> GetAllAsync();
    Task<Project?> GetByIdAsync(int id);
    Task<Project> CreateAsync(Project project);
    Task<Project> UpdateAsync(Project project);
    Task DeleteAsync(int id);
}
