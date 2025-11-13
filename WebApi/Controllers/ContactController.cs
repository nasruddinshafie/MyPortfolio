using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Application.Features.Contact.DTOs;
using Application.Features.Contact.Queries;
using Application.Features.Contact.Commands;
using MediatR;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactController : ControllerBase
{
    private readonly IMediator _mediator;

    public ContactController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Submit a contact form (Public endpoint)
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ContactDto>> CreateContact([FromBody] CreateContactDto createContactDto)
    {
        var command = new CreateContactCommand(createContactDto);
        var contact = await _mediator.Send(command);

        return CreatedAtAction(nameof(GetContactById), new { id = contact.Id }, contact);
    }

    /// <summary>
    /// Get all contact messages (Admin only)
    /// </summary>
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<ContactDto>>> GetAllContacts()
    {
        var contacts = await _mediator.Send(new GetAllContactsQuery());
        return Ok(contacts);
    }

    /// <summary>
    /// Get contact message by ID (Admin only)
    /// </summary>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<ContactDto>> GetContactById(int id)
    {
        var contact = await _mediator.Send(new GetContactByIdQuery(id));

        if (contact == null)
            return NotFound();

        return Ok(contact);
    }

    /// <summary>
    /// Mark contact message as read (Admin only)
    /// </summary>
    [HttpPatch("{id}/mark-read")]
    [Authorize]
    public async Task<ActionResult> MarkAsRead(int id)
    {
        var result = await _mediator.Send(new MarkContactAsReadCommand(id));

        if (!result)
            return NotFound();

        return NoContent();
    }
}
