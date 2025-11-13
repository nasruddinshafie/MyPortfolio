using Portfolio.UI.DTOs.Contact;
using System.Net.Http.Json;
using System.Text.Json;

namespace Portfolio.UI.Services;

public class ContactService : IContactService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public ContactService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<ContactDto> SubmitContactAsync(CreateContactDto contactDto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/contact", contactDto);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ContactDto>(_jsonOptions);
            return result ?? throw new InvalidOperationException("Failed to deserialize contact response");
        }
        catch (HttpRequestException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"Error submitting contact form: {ex.Message}", ex);
        }
    }

    public async Task<IEnumerable<ContactDto>> GetAllContactsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/contact");
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<IEnumerable<ContactDto>>(_jsonOptions);
            return result ?? new List<ContactDto>();
        }
        catch (HttpRequestException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"Error fetching contacts: {ex.Message}", ex);
        }
    }

    public async Task<ContactDto?> GetContactByIdAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/contact/{id}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ContactDto>(_jsonOptions);
            return result;
        }
        catch (HttpRequestException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"Error fetching contact: {ex.Message}", ex);
        }
    }

    public async Task MarkAsReadAsync(int id)
    {
        try
        {
            var response = await _httpClient.PatchAsync($"api/contact/{id}/mark-read", null);
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"Error marking contact as read: {ex.Message}", ex);
        }
    }
}
