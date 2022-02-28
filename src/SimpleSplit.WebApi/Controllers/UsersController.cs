using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleSplit.Application.Base;
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
        public async Task<ActionResult> LoginUser([FromBody] LoginUser request)
        {
            var response = await _mediator.Send(request);
            return response.ToActionResult();
        }

        /// <summary>
        /// Change or reset password.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("change-password"), AllowAnonymous]
        [SwaggerResponse(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangeUserPassword request)
        {
            var result = await _mediator.Send(request) as Result;
            return result.ToActionResult();
        }

        /// <summary>
        /// Refresh token.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("refresh-token")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(RefreshTokenResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshToken request)
        {
            var result = await _mediator.Send(request);
            return result.ToActionResult();
        }

        /// <summary>
        /// Update user profile.
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        [HttpPost, Route("profile")]
        [Consumes("multipart/form-data")]
        [SwaggerResponse(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SaveProfile([FromForm] Profile profile)
        {
            var request = new UpdateUserProfile
            {
                UserName    = profile.UserName,
                DisplayName = profile.DisplayName,
                Email       = profile.Email,
                UseGravatar = profile.UseGravatar,
                FileName    = profile.FileName,
                Image = profile.Image != null
                    ? await ImageToByteArray(profile.Image)
                    : Array.Empty<byte>(),
                PasswordInfo = profile.PasswordInfo != null
                    ? new ChangePasswordInfo
                    {
                        OldPassword = profile.PasswordInfo?.OldPassword,
                        NewPassword = profile.PasswordInfo?.NewPassword
                    }
                    : null
            };
            var response = await _mediator.Send(request);
            return response.ToActionResult();
        }

        /// <summary>
        /// Find user by email.
        /// </summary>
        /// <param name="email">E-mail</param>
        /// <response code="204">User with given email does not exist</response>
        /// <returns></returns>
        [HttpGet("by-email/{email}")]
        [SwaggerResponse(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status204NoContent, "User not found")]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByEmail([FromRoute] string email)
        {
            var response = await _mediator.Send(new SearchUserByEmail { Email = email });
            return response.ToActionResult();
        }
        
        private async Task<byte[]> ImageToByteArray(IFormFile formFile)
        {
            await using var memoryStream = new MemoryStream();
            await formFile.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }
    }

    public class Profile
    {
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string FileName { get; set; }
        public bool UseGravatar { get; set; }
        public IFormFile Image { get; set; }
        public ChangePasswordInfo PasswordInfo { get; set; }

        public class ChangePasswordInfo
        {
            public string OldPassword { get; set; }
            public string NewPassword { get; set; }
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
                    Name    = "normal",
                    Summary = "Simple user",
                    Value = new LoginUser
                    {
                        UserName = "userName",
                        Password = "password"
                    }
                },
                new SwaggerExample<LoginUser>()
                {
                    Name    = "systemAdmin",
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