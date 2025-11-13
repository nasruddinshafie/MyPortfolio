using Domain.Entities;

namespace Application.Common.Interfaces;

public interface IContactRepository
{
    Task<Contact> CreateAsync(Contact contact);
    Task<IEnumerable<Contact>> GetAllAsync();
    Task<Contact?> GetByIdAsync(int id);
    Task<Contact> UpdateAsync(Contact contact);
    Task DeleteAsync(int id);
}
