namespace Portfolio.UI.DTOs;

public class UpdateProjectDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? DetailedDescription { get; set; }
    public List<string> Technologies { get; set; } = new();
    public string? ProjectUrl { get; set; }
    public string? GitHubUrl { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }
}
