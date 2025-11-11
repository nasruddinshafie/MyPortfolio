using Portfolio.UI.DTOs;
using System.Text.Json;

namespace Portfolio.UI.Services;

public class BioService : IBioService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public BioService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<BioDto?> GetBioAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/bio");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<BioDto>(json, _jsonOptions);
            }
            
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            
            throw new HttpRequestException($"Error fetching bio: {response.StatusCode}");
        }
        catch (HttpRequestException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"Error fetching bio: {ex.Message}", ex);
        }
    }

    public async Task<BioDto> CreateBioAsync(CreateBioDto createBioDto)
    {
        try
        {
            var json = JsonSerializer.Serialize(createBioDto);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("api/bio", content);
            
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<BioDto>(responseJson, _jsonOptions);
                return result ?? throw new InvalidOperationException("Failed to deserialize created bio");
            }
            
            throw new HttpRequestException($"Error creating bio: {response.StatusCode}");
        }
        catch (HttpRequestException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"Error creating bio: {ex.Message}", ex);
        }
    }

    public async Task<BioDto> UpdateBioAsync(int id, UpdateBioDto updateBioDto)
    {
        try
        {
            var json = JsonSerializer.Serialize(updateBioDto);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PutAsync($"api/bio/{id}", content);
            
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<BioDto>(responseJson, _jsonOptions);
                return result ?? throw new InvalidOperationException("Failed to deserialize updated bio");
            }
            
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new InvalidOperationException("Bio not found");
            }
            
            throw new HttpRequestException($"Error updating bio: {response.StatusCode}");
        }
        catch (HttpRequestException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"Error updating bio: {ex.Message}", ex);
        }
    }
}