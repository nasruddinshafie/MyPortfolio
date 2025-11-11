using Application.Features.Bio.DTOs;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.Bio.Queries;

public record GetBioQuery : IRequest<BioDto?>;

public class GetBioQueryHandler : IRequestHandler<GetBioQuery, BioDto?>
{
    private readonly IBioRepository _bioRepository;

    public GetBioQueryHandler(IBioRepository bioRepository)
    {
        _bioRepository = bioRepository;
    }

    public async Task<BioDto?> Handle(GetBioQuery request, CancellationToken cancellationToken)
    {
        var bio = await _bioRepository.GetAsync();
        
        if (bio == null)
            return null;

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