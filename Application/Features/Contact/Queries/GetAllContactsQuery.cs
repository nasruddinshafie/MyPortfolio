using Application.Features.Contact.DTOs;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.Contact.Queries;

public record GetAllContactsQuery : IRequest<IEnumerable<ContactDto>>;

public class GetAllContactsQueryHandler : IRequestHandler<GetAllContactsQuery, IEnumerable<ContactDto>>
{
    private readonly IContactRepository _contactRepository;

    public GetAllContactsQueryHandler(IContactRepository contactRepository)
    {
        _contactRepository = contactRepository;
    }

    public async Task<IEnumerable<ContactDto>> Handle(GetAllContactsQuery request, CancellationToken cancellationToken)
    {
        var contacts = await _contactRepository.GetAllAsync();

        return contacts.Select(c => new ContactDto
        {
            Id = c.Id,
            Name = c.Name,
            Email = c.Email,
            Subject = c.Subject,
            Message = c.Message,
            IsRead = c.IsRead,
            CreatedAt = c.CreatedAt
        });
    }
}
