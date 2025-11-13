using Application.Common.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Features.Contact.Repositories;

public class ContactRepository : IContactRepository
{
    private readonly PortfolioDbContext _context;

    public ContactRepository(PortfolioDbContext context)
    {
        _context = context;
    }

    public async Task<Domain.Entities.Contact> CreateAsync(Domain.Entities.Contact contact)
    {
        _context.Contacts.Add(contact);
        await _context.SaveChangesAsync();
        return contact;
    }

    public async Task<IEnumerable<Domain.Entities.Contact>> GetAllAsync()
    {
        return await _context.Contacts
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<Domain.Entities.Contact?> GetByIdAsync(int id)
    {
        return await _context.Contacts.FindAsync(id);
    }

    public async Task<Domain.Entities.Contact> UpdateAsync(Domain.Entities.Contact contact)
    {
        _context.Contacts.Update(contact);
        await _context.SaveChangesAsync();
        return contact;
    }

    public async Task DeleteAsync(int id)
    {
        var contact = await _context.Contacts.FindAsync(id);
        if (contact != null)
        {
            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();
        }
    }
}
