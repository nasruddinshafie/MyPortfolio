using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Application.Features.Bio.DTOs;
using Application.Features.Bio.Queries;
using Application.Features.Bio.Commands;
using MediatR;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BioController : ControllerBase
{
    private readonly IMediator _mediator;

    public BioController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<BioDto>> GetBio()
    {
        var bio = await _mediator.Send(new GetBioQuery());
        
        if (bio == null)
            return NotFound();

        return Ok(bio);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<BioDto>> CreateBio([FromBody] CreateBioDto createBioDto)
    {
        var command = new CreateBioCommand(createBioDto);
        var bio = await _mediator.Send(command);
        
        return CreatedAtAction(nameof(GetBio), new { id = bio.Id }, bio);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<BioDto>> UpdateBio(int id, [FromBody] UpdateBioDto updateBioDto)
    {
        updateBioDto.Id = id;
        var command = new UpdateBioCommand(updateBioDto);
        
        try
        {
            var bio = await _mediator.Send(command);
            return Ok(bio);
        }
        catch (ArgumentException)
        {
            return NotFound();
        }
    }
}