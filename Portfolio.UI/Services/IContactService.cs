using Portfolio.UI.DTOs.Contact;

namespace Portfolio.UI.Services;

public interface IContactService
{
    Task<ContactDto> SubmitContactAsync(CreateContactDto contactDto);
    Task<IEnumerable<ContactDto>> GetAllContactsAsync();
    Task<ContactDto?> GetContactByIdAsync(int id);
    Task MarkAsReadAsync(int id);
}
