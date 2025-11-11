using Application.Common.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Features.Projects.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly PortfolioDbContext _context;

    public ProjectRepository(PortfolioDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Project>> GetAllAsync()
    {
        return await _context.Projects
            .OrderBy(p => p.DisplayOrder)
            .ToListAsync();
    }

    public async Task<Project?> GetByIdAsync(int id)
    {
        return await _context.Projects.FindAsync(id);
    }

    public async Task<Project> CreateAsync(Project project)
    {
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
        return project;
    }

    public async Task<Project> UpdateAsync(Project project)
    {
        _context.Projects.Update(project);
        await _context.SaveChangesAsync();
        return project;
    }

    public async Task DeleteAsync(int id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project != null)
        {
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
        }
    }
}
