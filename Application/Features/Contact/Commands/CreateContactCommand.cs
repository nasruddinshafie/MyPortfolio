using Application.Features.Contact.DTOs;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.Contact.Commands;

public record CreateContactCommand(CreateContactDto ContactDto) : IRequest<ContactDto>;

public class CreateContactCommandHandler : IRequestHandler<CreateContactCommand, ContactDto>
{
    private readonly IContactRepository _contactRepository;

    public CreateContactCommandHandler(IContactRepository contactRepository)
    {
        _contactRepository = contactRepository;
    }

    public async Task<ContactDto> Handle(CreateContactCommand request, CancellationToken cancellationToken)
    {
        var contact = new Domain.Entities.Contact
        {
            Name = request.ContactDto.Name,
            Email = request.ContactDto.Email,
            Subject = request.ContactDto.Subject,
            Message = request.ContactDto.Message,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        var createdContact = await _contactRepository.CreateAsync(contact);

        return new ContactDto
        {
            Id = createdContact.Id,
            Name = createdContact.Name,
            Email = createdContact.Email,
            Subject = createdContact.Subject,
            Message = createdContact.Message,
            IsRead = createdContact.IsRead,
            CreatedAt = createdContact.CreatedAt
        };
    }
}
