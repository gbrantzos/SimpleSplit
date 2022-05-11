using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleSplit.Application.Base;
using SimpleSplit.Application.Features.Buildings;
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

        /// <summary>
        /// Get a list with Buildings.
        /// </summary>
        /// <returns>List of <see cref="BuildingViewModel"/>.</returns>
        /// <param name="pageNumber" example="1">Page number.</param>
        /// <param name="pageSize" example="20">Page size.</param>
        /// <param name="sorting" example='["description"]'>Array of sorting information.</param>
        /// <param name="conditions" example='["description|starts|Γιαννιτσών"]'>
        /// Search conditions.
        /// <para>
        /// Conditions are in the form 'property|operator|value'.<br/>
        /// Supported operators are 'eq', 'neq', 'lt', 'lte', 'gt', 'gte', 'like', 'starts', 'ends', 'in', 'nin'.
        /// </para>
        /// </param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Returns a list of available buildings.</response>
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
            var searchRequest = new SearchBuildings()
            {
                SearchConditions = conditions ?? Array.Empty<string>(),
                SortingDetails   = sorting ?? Array.Empty<string>(),
                PageNumber       = pageNumber,
                PageSize         = pageSize
            };
            var response = await _mediator.Send(searchRequest, cancellationToken);
            return response.ToActionResult();
        }

        /// <summary>
        /// Create or modify a building entity.
        /// </summary>
        /// <param name="viewModel">Building model</param>
        /// <remarks>
        /// <para>
        /// When posting with `id=0`, a new Building is created. When `id` is not 0,
        /// you must also provide a `rowVersion` value for optimistic concurrency.
        /// </para>
        /// </remarks>
        /// <response code="200">Building created or modified</response>
        /// <response code="400"></response>
        /// <response code="500"></response>
        [HttpPost]
        public async Task<ActionResult> SaveBuilding([FromBody] BuildingViewModel viewModel)
        {
            var response = await _mediator.Send(new SaveBuilding {Model = viewModel});
            return response.ToActionResult();
        }
        
        /// <summary>
        /// Delete a building.
        /// </summary>
        /// When deleting, you must provide `rowVersion` value for optimistic concurrency.
        /// <param name="request"></param>
        /// <response code="200">Building deleted</response>
        /// <response code="400"></response>
        /// <response code="500"></response>
        [HttpDelete]
        public async Task<ActionResult> DeleteBuilding([FromBody] DeleteBuilding request)
        {
            var response = await _mediator.Send(request) as Result;
            return response.ToActionResult();
        }
    }
}
