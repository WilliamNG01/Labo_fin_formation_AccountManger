using Labo_fin_formation.DocumentManager.Models;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Labo_fin_formation.DocumentManager.CQRS_Commands.DocumentCommands;
using static Labo_fin_formation.DocumentManager.CQRS_Queries.DocumentQueries;
namespace Labo_fin_formation.DocumentManager.Controllers;

[ApiController]
[Route("[controller]")]
//[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class DocumentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public DocumentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("GetAllDocuments")]
    [Authorize(Roles = "DefaultUser", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> GetAllDocuments()
    {
        var result = await _mediator.Send(new GetALLDocumentsQuery());
        return Ok(result);
    }
    [HttpGet("GetFilteredDocuments")]
    [Authorize(Policy = "InternalDocumentsPolicy", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> GetDocuments([FromQuery] DocumentFilter filter)
    {
        var result = await _mediator.Send(new GetDocumentsQuery(filter));
        return Ok(result);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "MedicalRecordsPolicy", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> GetDocument(Guid id)
    {
        var result = await _mediator.Send(new GetDocumentByIdQuery(id));
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = "FullAccessPolicy", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> CreateDocument([FromBody] CreateDocumentCommand command)
    {
        var documentId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetDocument), new { id = documentId }, null);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "CoordinationPolicy", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> UpdateDocument(Guid id, [FromBody] UpdateDocumentCommand command)
    {
        if (id != command.Id) return BadRequest();
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "SupplyChainPolicy", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> DeleteDocument(Guid id)
    {
        await _mediator.Send(new DeleteDocumentCommand(id));
        return NoContent();
    }
}
