using Portfolio.UI.DTOs;

namespace Portfolio.UI.Services;

public interface IBioService
{
    Task<BioDto?> GetBioAsync();
    Task<BioDto> CreateBioAsync(CreateBioDto createBioDto);
    Task<BioDto> UpdateBioAsync(int id, UpdateBioDto updateBioDto);
}