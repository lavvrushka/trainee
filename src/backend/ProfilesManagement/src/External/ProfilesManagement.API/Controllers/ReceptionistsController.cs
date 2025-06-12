namespace ProfilesManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly IRequestHandler<CreatePatientRequest, Guid> _createHandler;
        private readonly IRequestHandler<UpdatePatientRequest, Unit> _updateHandler;

        public PatientsController(
            IRequestHandler<CreatePatientRequest, Guid> createHandler,
            IRequestHandler<UpdatePatientRequest, Unit> updateHandler)
        {
            _createHandler = createHandler;
            _updateHandler = updateHandler;
        }

        // POST: api/patients
        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] CreatePatientRequest request)
        {
            var id = await _createHandler.Handle(request, HttpContext.RequestAborted);
            return CreatedAtAction(null, id);
        }

        // PUT: api/patients/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePatientRequest request)
        {
            if (id != request.Id)
                return BadRequest("Route id and body id do not match.");

            await _updateHandler.Handle(request, HttpContext.RequestAborted);
            return NoContent();
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class ReceptionistsController : ControllerBase
    {
        private readonly IRequestHandler<CreateReceptionistRequest, Guid> _createHandler;
        private readonly IRequestHandler<UpdateReceptionistRequest, Unit> _updateHandler;

        public ReceptionistsController(
            IRequestHandler<CreateReceptionistRequest, Guid> createHandler,
            IRequestHandler<UpdateReceptionistRequest, Unit> updateHandler)
        {
            _createHandler = createHandler;
            _updateHandler = updateHandler;
        }

        // POST: api/receptionists
        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateReceptionistRequest request)
        {
            var id = await _createHandler.Handle(request, HttpContext.RequestAborted);
            return CreatedAtAction(null, id);
        }

        // PUT: api/receptionists/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateReceptionistRequest request)
        {
            if (id != request.Id)
                return BadRequest("Route id and body id do not match.");

            await _updateHandler.Handle(request, HttpContext.RequestAborted);
            return NoContent();
        }
    }
}
