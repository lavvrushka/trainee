using DocumentsBusinessLogic.DTOs.Documents;
using DocumentsBusinessLogic.UseCases.DocumentsUseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyMediator.Interfaces;
namespace DocumentsAPI.Controllers;

[Route("api/document")]
[ApiController]
[Authorize]
public class DocumentController : ControllerBase
{
    private readonly IMediator _mediator;
    public DocumentController(IMediator mediator) => _mediator = mediator;

    [HttpPost("create")]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateDocumentRequest request)
    {
        var id = await _mediator.Send(request);

        return Ok(id);
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] UpdateDocumentRequest request)
    {
        await _mediator.Send(request);

        return Ok();
    }

    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<DocumentDto>>> GetAll()
    {
        var documents = await _mediator.Send(new GetAllDocumentsRequest());

        return Ok(documents);
    }

    [HttpDelete("soft-delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new SoftDeleteDocumentRequest(id));

        return NoContent();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DocumentDto>> GetById(Guid id)
    {
        var dto = await _mediator.Send(new GetDocumentByIdRequest(id));

        return Ok(dto);
    }
}
