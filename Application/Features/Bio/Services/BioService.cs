using Application.Features.Bio.DTOs;
using Application.Common.Interfaces;
using Domain.Entities;

namespace Application.Features.Bio.Services;

public class BioService : IBioService
{
    private readonly IBioRepository _bioRepository;

    public BioService(IBioRepository bioRepository)
    {
        _bioRepository = bioRepository;
    }

    public async Task<BioDto?> GetBioAsync()
    {
        var bio = await _bioRepository.GetAsync();
        
        if (bio == null)
            return null;

        return MapToDto(bio);
    }

    public async Task<BioDto> CreateBioAsync(CreateBioDto createBioDto)
    {
        var bio = new Domain.Entities.Bio
        {
            FullName = createBioDto.FullName,
            Title = createBioDto.Title,
            Summary = createBioDto.Summary,
            DetailedDescription = createBioDto.DetailedDescription,
            Email = createBioDto.Email,
            Phone = createBioDto.Phone,
            Location = createBioDto.Location,
            LinkedInUrl = createBioDto.LinkedInUrl,
            GitHubUrl = createBioDto.GitHubUrl,
            WebsiteUrl = createBioDto.WebsiteUrl,
            ProfileImageUrl = createBioDto.ProfileImageUrl,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdBio = await _bioRepository.CreateAsync(bio);
        return MapToDto(createdBio);
    }

    public async Task<BioDto> UpdateBioAsync(UpdateBioDto updateBioDto)
    {
        var existingBio = await _bioRepository.GetAsync();
        if (existingBio == null)
        {
            throw new ArgumentException("Bio not found");
        }

        existingBio.FullName = updateBioDto.FullName;
        existingBio.Title = updateBioDto.Title;
        existingBio.Summary = updateBioDto.Summary;
        existingBio.DetailedDescription = updateBioDto.DetailedDescription;
        existingBio.Email = updateBioDto.Email;
        existingBio.Phone = updateBioDto.Phone;
        existingBio.Location = updateBioDto.Location;
        existingBio.LinkedInUrl = updateBioDto.LinkedInUrl;
        existingBio.GitHubUrl = updateBioDto.GitHubUrl;
        existingBio.WebsiteUrl = updateBioDto.WebsiteUrl;
        existingBio.ProfileImageUrl = updateBioDto.ProfileImageUrl;
        existingBio.UpdatedAt = DateTime.UtcNow;

        var updatedBio = await _bioRepository.UpdateAsync(existingBio);
        return MapToDto(updatedBio);
    }

    private static BioDto MapToDto(Domain.Entities.Bio bio)
    {
        return new BioDto
        {
            Id = bio.Id,
            FullName = bio.FullName,
            Title = bio.Title,
            Summary = bio.Summary,
            DetailedDescription = bio.DetailedDescription,
            Email = bio.Email,
            Phone = bio.Phone,
            Location = bio.Location,
            LinkedInUrl = bio.LinkedInUrl,
            GitHubUrl = bio.GitHubUrl,
            WebsiteUrl = bio.WebsiteUrl,
            ProfileImageUrl = bio.ProfileImageUrl,
            CreatedAt = bio.CreatedAt,
            UpdatedAt = bio.UpdatedAt
        };
    }
}