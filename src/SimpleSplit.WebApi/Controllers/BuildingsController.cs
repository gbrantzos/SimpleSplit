using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleSplit.Application.Base;
using SimpleSplit.Application.Features.Buildings;
using SimpleSplit.Domain.Features.Buildings;
using Swashbuckle.AspNetCore.Annotations;

namespace SimpleSplit.WebApi.Controllers
{
    [ApiController, Route("[controller]"), Authorize]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class BuildingsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BuildingsController(IMediator mediator)
            => _mediator = mediator;

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(PagedResult<>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetAll([FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = -1,
            [FromQuery] string[] sorting = null,
            [FromQuery] string[] conditions = null,
            CancellationToken cancellationToken = default)

        {
            await Task.CompletedTask;
            var toReturn = new PagedResult<BuildingViewModel>
            {
                CurrentPage = 1,
                PageSize    = 1,
                TotalRows   = 1,
                Rows = new List<BuildingViewModel>
                {
                    Building.ForTests().Adapt<BuildingViewModel>()
                }
            };
            return Ok(toReturn);
        }
    }
}