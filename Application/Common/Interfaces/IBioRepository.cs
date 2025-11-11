using Domain.Entities;

namespace Application.Common.Interfaces;

public interface IBioRepository
{
    Task<Bio?> GetAsync();
    Task<Bio> CreateAsync(Bio bio);
    Task<Bio> UpdateAsync(Bio bio);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync();
}