using System.Text.Json;
using System.Text.Json.Nodes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleSplit.Application.Base;
using SimpleSplit.Application.Features.Expenses;
using SimpleSplit.Application.Services;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace SimpleSplit.WebApi.Controllers
{
    // https://www.dotnetnakama.com/blog/enriched-web-api-documentation-using-swagger-openapi-in-asp-dotnet-core/

    /// <summary>
    /// Expenses controller.
    /// </summary>
    [ApiController, Route("[controller]"), Authorize]
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
        /// <param name="conditions" example='["description|starts|Καθαριότητα"]'>
        /// Search conditions.
        /// <para>
        /// Conditions are in the form 'property|operator|value'.<br/>
        /// Supported operators are 'eq', 'neq', 'lt', 'lte', 'gt', 'gte', 'like', 'starts', 'ends', 'in', 'nin'.
        /// </para>
        /// </param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Returns a list of available expenses.</response>
        [HttpGet]
        [SwaggerResponseExample(200, typeof(ExpensesGetAllExamples))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(PagedResult<ExpenseViewModel>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetAll([FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = -1,
            [FromQuery] string[] sorting = null,
            [FromQuery] string[] conditions = null,
            CancellationToken cancellationToken = default)
        {
            var searchRequest = new SearchExpenses
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
        /// Get Expense by ID.
        /// </summary>
        /// <param name="id">Expense ID.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Requested <see cref="ExpenseViewModel"/></returns>
        /// <response code="200">Returns Expense with requested ID.</response>
        [HttpGet("{id:long}")]
        [SwaggerResponseExample(200, typeof(ExpensesGetByIDExamples))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(ExpenseViewModel))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetByID(long id, CancellationToken cancellationToken = default)
        {
            var response = await _mediator.Send(new GetExpense { ID = id }, cancellationToken);
            return response.ToActionResult();
        }

        /// <summary>
        /// Create or modify an expense entity.
        /// </summary>
        /// <param name="viewModel">Expense model</param>
        /// <remarks>
        /// <para>
        /// Sample request:
        ///
        ///     POST /Expenses
        ///     {
        ///         "id": 0,
        ///         "description": "Sample expense",
        ///         "amount": 32.45,
        ///         "forOwner": false
        ///     }
        ///
        /// </para>
        /// <para>
        /// When posting with `id=0`, a new Expense is created. When `id` is not 0,
        /// you must also provide a `rowVersion` value for optimistic concurrency.
        /// </para>
        /// </remarks>
        /// <response code="200">Expense created or modified</response>
        /// <response code="400"></response>
        /// <response code="500"></response>
        [HttpPost]
        public async Task<ActionResult> SaveExpense([FromBody] ExpenseViewModel viewModel)
        {
            var response = await _mediator.Send(new SaveExpense { Model = viewModel });
            return response.ToActionResult();
        }

        /// <summary>
        /// Delete an expense.
        /// </summary>
        /// ## Sample request:
        ///
        ///     DELETE /expenses
        ///     {
        ///         "id": 12,
        ///         "rowVersion": 2
        ///     }
        ///
        /// When deleting, you must provide `rowVersion` value for optimistic concurrency.
        /// <param name="request"></param>
        /// <response code="200">Expense deleted</response>
        /// <response code="400"></response>
        /// <response code="500"></response>
        [HttpDelete]
        public async Task<ActionResult> DeleteExpense([FromBody] DeleteExpense request)
        {
            var response = await _mediator.Send(request) as Result;
            return response.ToActionResult();
        }

        /// <summary>
        /// Advanced search of Expense.
        /// </summary>
        /// <returns>List of <see cref="ExpenseViewModel"/>.</returns>
        /// <param name="pageNumber" example="1">Page number.</param>
        /// <param name="pageSize" example="20">Page size.</param>
        /// <param name="sorting" example='["-enteredAt","description"]'>Array of sorting information.</param>
        /// <param name="cancellationToken"></param>
        /// <response code="200">Returns a list of available expenses.</response>
        [SwaggerResponseExample(200, typeof(ExpensesGetAllExamples))]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(PagedResult<ExpenseViewModel>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        [HttpPost("advanced-search")]
        public async Task<ActionResult> GetAll([FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = -1,
            [FromQuery] string[] sorting = null,
            CancellationToken cancellationToken = default)
        {
            using var reader = new StreamReader(Request.Body);
            var rawBody = await reader.ReadToEndAsync();

            var rootGroup = ParseConditionGroup(rawBody);
            var searchRequest = new SearchExpenses
            {
                SearchConditions = Array.Empty<string>(),
                SortingDetails   = sorting ?? Array.Empty<string>(),
                PageNumber       = pageNumber,
                PageSize         = pageSize,
                AdvancedSearch   = rootGroup
            };
            var response = await _mediator.Send(searchRequest, cancellationToken);
            return response.ToActionResult();
        }

        /// <summary>
        /// Mass update expenses category.
        /// </summary>
        /// <param name="request"></param>
        /// <response code="200">Expenses category updated</response>
        /// <response code="400"></response>
        /// <response code="500"></response>
        [HttpPost("update-category")]
        public async Task<ActionResult> UpdateCategory([FromBody] UpdateCategory request)
        {
            var response = await _mediator.Send(request);
            return response.ToActionResult();
        }

        private static ConditionGroup ParseConditionGroup(string rawBody)
        {
            // TODO In the future we must support parsing nested groups!
            var rootGroup = new ConditionGroup();
            var options = new JsonDocumentOptions { AllowTrailingCommas = true };

            using var document = JsonDocument.Parse(rawBody, options);
            foreach (var element in document.RootElement.EnumerateObject())
            {
                if (element.Name.Equals(nameof(ConditionGroup.Grouping), StringComparison.CurrentCultureIgnoreCase))
                    rootGroup.Grouping = Enum.Parse<ConditionGroup.GroupingOperator>(element.Value.ToString(), true);
                if (element.Name.Equals(nameof(ConditionGroup.Conditions), StringComparison.CurrentCultureIgnoreCase))
                {
                    // Parse conditions
                    foreach (var conditionJson in element.Value.EnumerateArray())
                    {
                        rootGroup.Conditions.Add(new Condition
                        {
                            Property = conditionJson.GetProperty("property").GetString(),
                            Operator = conditionJson.GetProperty("operator").GetString(),
                            Value = conditionJson.GetProperty("value").ValueKind == JsonValueKind.Array
                                ? conditionJson.GetProperty("value")
                                    .EnumerateArray()
                                    .Select(i => i.ToString())
                                    .Aggregate(String.Empty, (curr, next) => curr + "," + next)
                                    .TrimStart(',')
                                : conditionJson.GetProperty("value").GetString()
                        });
                    }
                }
            }

            return rootGroup;
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
                    ID          = 1,
                    Description = "Καθαριότητα Νοεμβρίου - Δεκεμβρίου",
                    EnteredAt   = new DateTime(2021, 10, 21),
                    Amount      = 116
                },
                new ExpenseViewModel
                {
                    ID          = 2,
                    Description = "ΔΕΗ Νοεμβρίου - Δεκεμβρίου",
                    EnteredAt   = new DateTime(2021, 10, 15),
                    Amount      = 74
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
                ID            = 12,
                RowVersion    = 1,
                Description   = "Expesne to share",
                EnteredAt     = DateTime.Now,
                Amount        = 60.32m,
                IsOwnerCharge = false,
            };
        }
    }
}