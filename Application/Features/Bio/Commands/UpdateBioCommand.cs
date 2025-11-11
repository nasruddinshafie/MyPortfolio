using Application.Features.Bio.DTOs;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.Bio.Commands;

public record UpdateBioCommand(UpdateBioDto BioDto) : IRequest<BioDto>;

public class UpdateBioCommandHandler : IRequestHandler<UpdateBioCommand, BioDto>
{
    private readonly IBioRepository _bioRepository;

    public UpdateBioCommandHandler(IBioRepository bioRepository)
    {
        _bioRepository = bioRepository;
    }

    public async Task<BioDto> Handle(UpdateBioCommand request, CancellationToken cancellationToken)
    {
        var existingBio = await _bioRepository.GetAsync();
        if (existingBio == null)
        {
            throw new ArgumentException("Bio not found");
        }

        existingBio.FullName = request.BioDto.FullName;
        existingBio.Title = request.BioDto.Title;
        existingBio.Summary = request.BioDto.Summary;
        existingBio.DetailedDescription = request.BioDto.DetailedDescription;
        existingBio.Email = request.BioDto.Email;
        existingBio.Phone = request.BioDto.Phone;
        existingBio.Location = request.BioDto.Location;
        existingBio.LinkedInUrl = request.BioDto.LinkedInUrl;
        existingBio.GitHubUrl = request.BioDto.GitHubUrl;
        existingBio.WebsiteUrl = request.BioDto.WebsiteUrl;
        existingBio.ProfileImageUrl = request.BioDto.ProfileImageUrl;
        existingBio.UpdatedAt = DateTime.UtcNow;

        var updatedBio = await _bioRepository.UpdateAsync(existingBio);

        return new BioDto
        {
            Id = updatedBio.Id,
            FullName = updatedBio.FullName,
            Title = updatedBio.Title,
            Summary = updatedBio.Summary,
            DetailedDescription = updatedBio.DetailedDescription,
            Email = updatedBio.Email,
            Phone = updatedBio.Phone,
            Location = updatedBio.Location,
            LinkedInUrl = updatedBio.LinkedInUrl,
            GitHubUrl = updatedBio.GitHubUrl,
            WebsiteUrl = updatedBio.WebsiteUrl,
            ProfileImageUrl = updatedBio.ProfileImageUrl,
            CreatedAt = updatedBio.CreatedAt,
            UpdatedAt = updatedBio.UpdatedAt
        };
    }
}