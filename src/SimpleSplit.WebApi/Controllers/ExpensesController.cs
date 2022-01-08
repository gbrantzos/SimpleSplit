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
        /// <returns>List of <see cref="ExpenseViewModel"/></returns>
        /// <param name="pageNumber" example="1">Page number</param>
        /// <param name="pageSize" example="20">Page size</param>
        /// <param name="sorting" example='["enteredAt,DESC","description"]'>Array of sorting information</param>
        /// <param name="queryParameters">Additional query string paramaters, used to pass search details</param>
        /// <response code="200">Returns a list of available expenses.</response>
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(PagedResult<ExpenseViewModel>))]
        [SwaggerResponseExample(200, typeof(ExpensesGetAllExamples))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetAll([FromQuery]int pageNumber = 1,
            [FromQuery] int pageSize = -1,
            [FromQuery] string[] sorting = null,
            [FromQuery] IDictionary<string, string[]> queryParameters = null)
        {
            var searchRequest = new SearchExpenses
            {
                SortingDetails = sorting,
                PageNumber     = pageNumber,
                PageSize       = pageSize
            };
            var response = await _mediator.Send(searchRequest);
            if (response.HasErrors)
                return BadRequest(response.AllErrors());

            return Ok(response.Data);
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
}