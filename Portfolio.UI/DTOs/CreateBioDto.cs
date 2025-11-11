namespace Portfolio.UI.DTOs;

public class CreateBioDto
{
    public string FullName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string? DetailedDescription { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Location { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? GitHubUrl { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? ProfileImageUrl { get; set; }
}
