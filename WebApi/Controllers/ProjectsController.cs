using Microsoft.AspNetCore.Mvc;
using Application.Features.Projects.DTOs;
using Application.Features.Projects.Queries;
using Application.Features.Projects.Commands;
using MediatR;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProjectsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectDto>>> GetAllProjects()
    {
        var projects = await _mediator.Send(new GetAllProjectsQuery());
        return Ok(projects);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectDto>> GetProjectById(int id)
    {
        var project = await _mediator.Send(new GetProjectByIdQuery(id));

        if (project == null)
            return NotFound();

        return Ok(project);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectDto>> CreateProject([FromBody] CreateProjectDto createProjectDto)
    {
        var command = new CreateProjectCommand(createProjectDto);
        var project = await _mediator.Send(command);

        return CreatedAtAction(nameof(GetProjectById), new { id = project.Id }, project);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProjectDto>> UpdateProject(int id, [FromBody] UpdateProjectDto updateProjectDto)
    {
        updateProjectDto.Id = id;
        var command = new UpdateProjectCommand(updateProjectDto);

        try
        {
            var project = await _mediator.Send(command);
            return Ok(project);
        }
        catch (ArgumentException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProject(int id)
    {
        var command = new DeleteProjectCommand(id);
        await _mediator.Send(command);

        return NoContent();
    }
}
