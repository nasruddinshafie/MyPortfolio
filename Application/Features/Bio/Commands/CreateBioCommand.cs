using Application.Features.Bio.DTOs;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.Bio.Commands;

public record CreateBioCommand(CreateBioDto BioDto) : IRequest<BioDto>;

public class CreateBioCommandHandler : IRequestHandler<CreateBioCommand, BioDto>
{
    private readonly IBioRepository _bioRepository;

    public CreateBioCommandHandler(IBioRepository bioRepository)
    {
        _bioRepository = bioRepository;
    }

    public async Task<BioDto> Handle(CreateBioCommand request, CancellationToken cancellationToken)
    {
        var bio = new Domain.Entities.Bio
        {
            FullName = request.BioDto.FullName,
            Title = request.BioDto.Title,
            Summary = request.BioDto.Summary,
            DetailedDescription = request.BioDto.DetailedDescription,
            Email = request.BioDto.Email,
            Phone = request.BioDto.Phone,
            Location = request.BioDto.Location,
            LinkedInUrl = request.BioDto.LinkedInUrl,
            GitHubUrl = request.BioDto.GitHubUrl,
            WebsiteUrl = request.BioDto.WebsiteUrl,
            ProfileImageUrl = request.BioDto.ProfileImageUrl,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdBio = await _bioRepository.CreateAsync(bio);

        return new BioDto
        {
            Id = createdBio.Id,
            FullName = createdBio.FullName,
            Title = createdBio.Title,
            Summary = createdBio.Summary,
            DetailedDescription = createdBio.DetailedDescription,
            Email = createdBio.Email,
            Phone = createdBio.Phone,
            Location = createdBio.Location,
            LinkedInUrl = createdBio.LinkedInUrl,
            GitHubUrl = createdBio.GitHubUrl,
            WebsiteUrl = createdBio.WebsiteUrl,
            ProfileImageUrl = createdBio.ProfileImageUrl,
            CreatedAt = createdBio.CreatedAt,
            UpdatedAt = createdBio.UpdatedAt
        };
    }
}