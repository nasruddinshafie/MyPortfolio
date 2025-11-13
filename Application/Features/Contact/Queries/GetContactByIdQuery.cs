using Application.Features.Contact.DTOs;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.Contact.Queries;

public record GetContactByIdQuery(int Id) : IRequest<ContactDto?>;

public class GetContactByIdQueryHandler : IRequestHandler<GetContactByIdQuery, ContactDto?>
{
    private readonly IContactRepository _contactRepository;

    public GetContactByIdQueryHandler(IContactRepository contactRepository)
    {
        _contactRepository = contactRepository;
    }

    public async Task<ContactDto?> Handle(GetContactByIdQuery request, CancellationToken cancellationToken)
    {
        var contact = await _contactRepository.GetByIdAsync(request.Id);

        if (contact == null)
            return null;

        return new ContactDto
        {
            Id = contact.Id,
            Name = contact.Name,
            Email = contact.Email,
            Subject = contact.Subject,
            Message = contact.Message,
            IsRead = contact.IsRead,
            CreatedAt = contact.CreatedAt
        };
    }
}
