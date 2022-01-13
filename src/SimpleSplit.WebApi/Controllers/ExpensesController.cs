using MediatR;
using Microsoft.AspNetCore.Mvc;
using SimpleSplit.Application.Base;
using SimpleSplit.Application.Features.Expenses;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace SimpleSplit.WebApi.Controllers
{
    // https://www.dotnetnakama.com/blog/enriched-web-api-documentation-using-swagger-openapi-in-asp-dotnet-core/

    /// <summary>
    /// Expenses controller.
    /// </summary>
    [ApiController, Route("[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class ExpensesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ExpensesController(IMediator mediator)
            => _mediator = mediator;

        /// <summary>
        /// Get a list with Expense items.
        /// </summary>
        /// <returns>List of <see cref="ExpenseViewModel"/>.</returns>
        /// <param name="pageNumber" example="1">Page number.</param>
        /// <param name="pageSize" example="20">Page size.</param>
        /// <param name="sorting" example='["-enteredAt","description"]'>Array of sorting information.</param>
        /// <param name="conditions" example='["amount|gte|50"]'>
        /// Search conditions.
        /// <para>
        /// Conditions are in the form 'property|operator|value'.<br/>
        /// Supported operators are 'eq', 'neq', 'lt', 'lte', 'gt', 'gte', 'like', 'starts', 'ends'.
        /// </para>
        /// </param>
        /// <response code="200">Returns a list of available expenses.</response>
        [HttpGet]
        [SwaggerResponseExample(200, typeof(ExpensesGetAllExamples))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(PagedResult<ExpenseViewModel>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetAll([FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = -1,
            [FromQuery] string[] sorting = null,
            [FromQuery] string[] conditions = null)
        {
            var searchRequest = new SearchExpenses
            {
                SearchConditions = conditions ?? Array.Empty<string>(),
                SortingDetails = sorting ?? Array.Empty<string>(),
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var response = await _mediator.Send(searchRequest);

            if (response.HasException)
                return StatusCode(StatusCodes.Status500InternalServerError, response.AllErrors());

            if (response.HasErrors)
                return BadRequest(response.AllErrors());

            return Ok(response.Value);
        }

        /// <summary>
        /// Get Expense by ID.
        /// </summary>
        /// <param name="id">Expense ID.</param>
        /// <returns>Requested <see cref="ExpenseViewModel"/></returns>
        /// <response code="200">Returns Expense with requested ID.</response>
        [HttpGet("{id:int}")]
        [SwaggerResponseExample(200, typeof(ExpensesGetByIDExamples))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ExpenseViewModel))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetByID(int id)
        {
            var response = await _mediator.Send(new GetExpense { ID = id });

            if (response.HasException)
                return StatusCode(StatusCodes.Status500InternalServerError, response.AllErrors());

            if (response.HasErrors)
                return BadRequest(response.AllErrors());

            return Ok(response.Value);
        }
    }

    public class ExpensesGetAllExamples : IExamplesProvider<IEnumerable<ExpenseViewModel>>
    {
        public IEnumerable<ExpenseViewModel> GetExamples()
        {
            return new List<ExpenseViewModel>
            {
                new ExpenseViewModel
                {
                    ID = 1,
                    Description = "Καθαριότητα Νοεμβρίου - Δεκεμβρίου",
                    EnteredAt = new DateTime(2021, 10, 21),
                    Amount = 116
                },
                new ExpenseViewModel
                {
                    ID = 2,
                    Description = "ΔΕΗ Νοεμβρίου - Δεκεμβρίου",
                    EnteredAt = new DateTime(2021, 10, 15),
                    Amount = 74
                }
            };
        }
    }

    public class ExpensesGetByIDExamples : IExamplesProvider<ExpenseViewModel>
    {
        public ExpenseViewModel GetExamples()
        {
            return new ExpenseViewModel
            {
                ID = 12,
                RowVersion = 1,
                Description = "Expesne to share",
                EnteredAt = DateTime.Now,
                Amount = 60.32m,
                IsOwnerCharge = false,
            };
        }
    }
}