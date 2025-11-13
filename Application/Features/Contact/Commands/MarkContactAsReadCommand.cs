using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.Contact.Commands;

public record MarkContactAsReadCommand(int Id) : IRequest<bool>;

public class MarkContactAsReadCommandHandler : IRequestHandler<MarkContactAsReadCommand, bool>
{
    private readonly IContactRepository _contactRepository;

    public MarkContactAsReadCommandHandler(IContactRepository contactRepository)
    {
        _contactRepository = contactRepository;
    }

    public async Task<bool> Handle(MarkContactAsReadCommand request, CancellationToken cancellationToken)
    {
        var contact = await _contactRepository.GetByIdAsync(request.Id);

        if (contact == null)
            return false;

        contact.IsRead = true;
        await _contactRepository.UpdateAsync(contact);

        return true;
    }
}
