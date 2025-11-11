using Application.Features.Bio.DTOs;

namespace Application.Features.Bio.Services;

public interface IBioService
{
    Task<BioDto?> GetBioAsync();
    Task<BioDto> CreateBioAsync(CreateBioDto createBioDto);
    Task<BioDto> UpdateBioAsync(UpdateBioDto updateBioDto);
}