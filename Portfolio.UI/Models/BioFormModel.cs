using System.ComponentModel.DataAnnotations;
using Portfolio.UI.DTOs;

namespace Portfolio.UI.Models;

public class BioFormModel
{
    [Required(ErrorMessage = "Full Name is required")]
    [StringLength(200, ErrorMessage = "Full Name must not exceed 200 characters")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, ErrorMessage = "Title must not exceed 200 characters")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Summary is required")]
    [StringLength(1000, ErrorMessage = "Summary must not exceed 1000 characters")]
    public string Summary { get; set; } = string.Empty;

    [StringLength(5000, ErrorMessage = "Detailed Description must not exceed 5000 characters")]
    public string? DetailedDescription { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(200, ErrorMessage = "Email must not exceed 200 characters")]
    public string Email { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "Phone must not exceed 50 characters")]
    public string? Phone { get; set; }

    [StringLength(200, ErrorMessage = "Location must not exceed 200 characters")]
    public string? Location { get; set; }

    [Url(ErrorMessage = "Invalid LinkedIn URL format")]
    [StringLength(500, ErrorMessage = "LinkedIn URL must not exceed 500 characters")]
    public string? LinkedInUrl { get; set; }

    [Url(ErrorMessage = "Invalid GitHub URL format")]
    [StringLength(500, ErrorMessage = "GitHub URL must not exceed 500 characters")]
    public string? GitHubUrl { get; set; }

    [Url(ErrorMessage = "Invalid Website URL format")]
    [StringLength(500, ErrorMessage = "Website URL must not exceed 500 characters")]
    public string? WebsiteUrl { get; set; }

    [Url(ErrorMessage = "Invalid Profile Image URL format")]
    [StringLength(500, ErrorMessage = "Profile Image URL must not exceed 500 characters")]
    public string? ProfileImageUrl { get; set; }
    
    public CreateBioDto ToCreateDto()
    {
        return new CreateBioDto
        {
            FullName = FullName,
            Title = Title,
            Summary = Summary,
            DetailedDescription = DetailedDescription,
            Email = Email,
            Phone = Phone,
            Location = Location,
            LinkedInUrl = LinkedInUrl,
            GitHubUrl = GitHubUrl,
            WebsiteUrl = WebsiteUrl,
            ProfileImageUrl = ProfileImageUrl
        };
    }
    
    public UpdateBioDto ToUpdateDto(int id)
    {
        return new UpdateBioDto
        {
            Id = id,
            FullName = FullName,
            Title = Title,
            Summary = Summary,
            DetailedDescription = DetailedDescription,
            Email = Email,
            Phone = Phone,
            Location = Location,
            LinkedInUrl = LinkedInUrl,
            GitHubUrl = GitHubUrl,
            WebsiteUrl = WebsiteUrl,
            ProfileImageUrl = ProfileImageUrl
        };
    }
    
    public static BioFormModel FromBioDto(BioDto bioDto)
    {
        return new BioFormModel
        {
            FullName = bioDto.FullName,
            Title = bioDto.Title,
            Summary = bioDto.Summary,
            DetailedDescription = bioDto.DetailedDescription,
            Email = bioDto.Email,
            Phone = bioDto.Phone,
            Location = bioDto.Location,
            LinkedInUrl = bioDto.LinkedInUrl,
            GitHubUrl = bioDto.GitHubUrl,
            WebsiteUrl = bioDto.WebsiteUrl,
            ProfileImageUrl = bioDto.ProfileImageUrl
        };
    }
}