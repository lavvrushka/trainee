using DocumentsBusinessLogic.DTOs.Images;
using DocumentsBusinessLogic.UseCases.ImagesUseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyMediator.Interfaces;
namespace DocumentsAPI.Controllers;

[Route("api/image")]
[ApiController]
[Authorize]
public class ImageController : ControllerBase
{
    private readonly IMediator _mediator;
    public ImageController(IMediator mediator) => _mediator = mediator;

    [HttpPost("upload")]
    public async Task<ActionResult<Guid>> Upload([FromForm] CreateImageRequest request)
    {
        var id = await _mediator.Send(request);

        return Ok(id);
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update([FromForm] UpdateImageRequest request)
    {
        await _mediator.Send(request);

        return Ok();
    }

    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<ImageDto>>> GetAll()
    {
        var images = await _mediator.Send(new GetAllImagesRequest());

        return Ok(images);
    }

    [HttpDelete("soft-delete/{id:guid}")]
    public async Task<IActionResult> SoftDelete(Guid id)
    {
        await _mediator.Send(new SoftDeleteImageRequest(id));

        return NoContent();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ImageDto>> GetById(Guid id)
    {
        var dto = await _mediator.Send(new GetImageByIdRequest(id));

        return Ok(dto);
    }
}
