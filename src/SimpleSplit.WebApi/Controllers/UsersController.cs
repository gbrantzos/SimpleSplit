using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleSplit.Application.Features.Security;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace SimpleSplit.WebApi.Controllers
{
    /// <summary>
    /// Users controller.
    /// </summary>
    [ApiController, Route("[controller]"), Authorize]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
            => _mediator = mediator;

        /// <summary>
        /// User login.
        /// </summary>
        /// <param name="request"></param>
        /// <response code="200">Returns user details along with JWT token.</response>
        /// <returns>After successful login an instance of see cref="LoginUserResponse"/></returns>
        [HttpPost, AllowAnonymous]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(LoginUserResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        [SwaggerRequestExample(typeof(LoginUser), typeof(LoginUserExamples))]
        public async Task<IActionResult> LoginUser([FromBody] LoginUser request)
        {
            var response = await _mediator.Send(request);

            if (response.HasException)
                return StatusCode(StatusCodes.Status500InternalServerError, response.AllErrors());

            if (response.HasErrors)
                return BadRequest(response.AllErrors());

            return Ok(response.Value);
        }
    }

    public class LoginUserExamples : IMultipleExamplesProvider<LoginUser>
    {
        public IEnumerable<SwaggerExample<LoginUser>> GetExamples()
        {
            return new[]
            {
                new SwaggerExample<LoginUser>()
                {
                    Name = "normal",
                    Summary = "Simple user",
                    Value = new LoginUser
                    {
                        UserName = "userName",
                        Password = "password"
                    }
                },
                new SwaggerExample<LoginUser>()
                {
                    Name = "systemAdmin",
                    Summary = "Internal Administrator",
                    Value = new LoginUser
                    {
                        UserName = "SimpleSplit",
                        Password = "search_on_logs"
                    }
                }
            };
        }
    }
}
