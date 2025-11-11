using Portfolio.UI.DTOs;
using System.Text.Json;

namespace Portfolio.UI.Services;

public class ProjectService : IProjectService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public ProjectService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<IEnumerable<ProjectDto>> GetAllProjectsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/projects");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var projects = JsonSerializer.Deserialize<List<ProjectDto>>(json, _jsonOptions);
                return projects ?? new List<ProjectDto>();
            }

            throw new HttpRequestException($"Error fetching projects: {response.StatusCode}");
        }
        catch (HttpRequestException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"Error fetching projects: {ex.Message}", ex);
        }
    }

    public async Task<ProjectDto?> GetProjectByIdAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/projects/{id}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ProjectDto>(json, _jsonOptions);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            throw new HttpRequestException($"Error fetching project: {response.StatusCode}");
        }
        catch (HttpRequestException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"Error fetching project: {ex.Message}", ex);
        }
    }

    public async Task<ProjectDto> CreateProjectAsync(CreateProjectDto createProjectDto)
    {
        try
        {
            var json = JsonSerializer.Serialize(createProjectDto);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/projects", content);

            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ProjectDto>(responseJson, _jsonOptions);
                return result ?? throw new InvalidOperationException("Failed to deserialize created project");
            }

            throw new HttpRequestException($"Error creating project: {response.StatusCode}");
        }
        catch (HttpRequestException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"Error creating project: {ex.Message}", ex);
        }
    }

    public async Task<ProjectDto> UpdateProjectAsync(int id, UpdateProjectDto updateProjectDto)
    {
        try
        {
            var json = JsonSerializer.Serialize(updateProjectDto);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"api/projects/{id}", content);

            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ProjectDto>(responseJson, _jsonOptions);
                return result ?? throw new InvalidOperationException("Failed to deserialize updated project");
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new InvalidOperationException("Project not found");
            }

            throw new HttpRequestException($"Error updating project: {response.StatusCode}");
        }
        catch (HttpRequestException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"Error updating project: {ex.Message}", ex);
        }
    }

    public async Task DeleteProjectAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/projects/{id}");

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Error deleting project: {response.StatusCode}");
            }
        }
        catch (HttpRequestException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"Error deleting project: {ex.Message}", ex);
        }
    }
}
