using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleSplit.Application.Base;
using SimpleSplit.Application.Features.Expenses;
using Swashbuckle.AspNetCore.Annotations;

namespace SimpleSplit.WebApi.Controllers
{
    /// <summary>
    /// Categories controller.
    /// </summary>
    [ApiController, Route("[controller]"), Authorize]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
            => _mediator = mediator;

        /// <summary>
        /// Get a list with Category items.
        /// </summary>
        /// <param name="pageNumber" example="1">Page number.</param>
        /// <param name="pageSize" example="20">Page size.</param>
        /// <param name="sorting" example='["-enteredAt","description"]'>Array of sorting information.</param>
        /// <param name="conditions" example='["description|starts|Καθαριότητα"]'>
        /// Search conditions.
        /// <para>
        /// Conditions are in the form 'property|operator|value'.<br/>
        /// Supported operators are 'eq', 'neq', 'lt', 'lte', 'gt', 'gte', 'like', 'starts', 'ends'.
        /// </para>
        /// </param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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
            var searchRequest = new SearchCategories()
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
        /// Create or modify a category entity.
        /// </summary>
        /// <param name="viewModel">Category model</param>
        /// <remarks>
        /// <para>
        /// Sample request:
        ///
        ///     POST /Categories
        ///     {
        ///         "id": 0,
        ///         "description": "Sample category",
        ///         "kind": 3
        ///     }
        ///
        /// </para>
        /// <para>
        /// When posting with `id=0`, a new Category is created. When `id` is not 0,
        /// you must also provide a `rowVersion` value for optimistic concurrency.
        /// </para>
        /// </remarks>
        /// <response code="200">Category created or modified</response>
        /// <response code="400"></response>
        /// <response code="500"></response>
        [HttpPost]
        public async Task<ActionResult> SaveCategory([FromBody] CategoryViewModel viewModel)
        {
            var response = await _mediator.Send(new SaveCategory { Model = viewModel });
            return response.ToActionResult();
        }
        
        /// <summary>
        /// Delete a category.
        /// </summary>
        /// ## Sample request:
        ///
        ///     DELETE /Categories
        ///     {
        ///         "id": 12,
        ///         "rowVersion": 2
        ///     }
        ///
        /// When deleting, you must provide `rowVersion` value for optimistic concurrency.
        /// <param name="request"></param>
        /// <response code="200">Category deleted</response>
        /// <response code="400"></response>
        /// <response code="500"></response>
        [HttpDelete]
        public async Task<ActionResult> DeleteCategory([FromBody] DeleteCategory request)
        {
            var response = await _mediator.Send(request) as Result;
            return response.ToActionResult();
        }
    }
}