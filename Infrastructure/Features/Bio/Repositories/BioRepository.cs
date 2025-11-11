using Application.Common.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Features.Bio.Repositories;

public class BioRepository : IBioRepository
{
    private readonly PortfolioDbContext _context;

    public BioRepository(PortfolioDbContext context)
    {
        _context = context;
    }

    public async Task<Domain.Entities.Bio?> GetAsync()
    {
        return await _context.Bios.FirstOrDefaultAsync();
    }

    public async Task<Domain.Entities.Bio> CreateAsync(Domain.Entities.Bio bio)
    {
        _context.Bios.Add(bio);
        await _context.SaveChangesAsync();
        return bio;
    }

    public async Task<Domain.Entities.Bio> UpdateAsync(Domain.Entities.Bio bio)
    {
        _context.Bios.Update(bio);
        await _context.SaveChangesAsync();
        return bio;
    }

    public async Task DeleteAsync(int id)
    {
        var bio = await _context.Bios.FindAsync(id);
        if (bio != null)
        {
            _context.Bios.Remove(bio);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync()
    {
        return await _context.Bios.AnyAsync();
    }
}