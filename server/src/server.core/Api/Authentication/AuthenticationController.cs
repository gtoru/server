using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using server.core.Api.Dto;
using server.core.Application;
using server.core.Domain.Error;
using server.core.Infrastructure;
using server.core.Infrastructure.Error;
using Swashbuckle.AspNetCore.Annotations;

namespace server.core.Api.Authentication
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/auth/v{version:apiVersion}")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticator _authenticator;
        private readonly ILogger<AuthenticationController> _log;

        public AuthenticationController(ILogger<AuthenticationController> log, IAuthenticator authenticator)
        {
            _log = log;
            _authenticator = authenticator;
        }

        [AllowAnonymous]
        [SwaggerOperation(
            Description = "User emails are unique",
            Summary = "Registers new user")]
        [SwaggerResponse(200, "User is created", typeof(OkResult))]
        [SwaggerResponse(409, "User with specified email already exists", typeof(ConflictResult))]
        [HttpPost("user/register")]
        public async Task<ActionResult> RegisterAsync(
            [FromBody] RegistrationRequest request,
            [FromServices] IUnitOfWork unitOfWork)
        {
            try
            {
                await AuthenticationManager.RegisterUserAsync(unitOfWork, request.Email, request.Password,
                    request.ExtractPersonalInfo());
            }
            catch (UserAlreadyExistsException)
            {
                _log.LogWarning("Trying to register user with existing email: {Email}", request.Email);
                return Conflict("user with specified email already exists");
            }

            return Ok();
        }

        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Authenticates user",
            Description = "Creates and returns JWT token with user info")]
        [SwaggerResponse(200, "Authentication successful", typeof(string))]
        [SwaggerResponse(403, "User not found or password is incorrect", typeof(ForbidResult))]
        [HttpPost("user/authenticate")]
        public async Task<ActionResult<string>> AuthenticateAsync(
            [FromBody] AuthenticationRequest request,
            [FromServices] IUnitOfWork unitOfWork)
        {
            using var scope =
                _log.BeginScope("Begin authentication for user with email: {Email}", request.Email);
            try
            {
                var token = await _authenticator.AuthenticateAsync(unitOfWork, request.Email, request.Password);

                return Ok(token);
            }
            catch (UserNotFoundException)
            {
                _log.LogWarning("Failed to find user with email: {Email}", request.Email);
                return Forbid();
            }
            catch (IncorrectPasswordException)
            {
                _log.LogWarning("Password is incorrect");
                return Forbid();
            }
        }

        [SwaggerOperation(
            Description = "Requires authentication",
            Summary = "Returns info about current user session")]
        [SwaggerResponse(200, "Authentication successful, info returned", typeof(SessionInfo))]
        [SwaggerResponse(401, "User unauthorized", typeof(UnauthorizedResult))]
        [SwaggerResponse(404,
            "Authentication token is valid, but user is not found. Sign of data corruption, unlikely to happen",
            typeof(NotFoundResult))]
        [HttpGet("sessions/my")]
        public async Task<ActionResult<SessionInfo>> GetSessionInfoAsync([FromServices] IUnitOfWork unitOfWork)
        {
            var userId = Guid.Parse(HttpContext.User.Claims.Single(c => c.Type == ClaimTypes.Name).Value);

            try
            {
                var (email, personalInfo) = await UserManager.GetUserInfoAsync(
                    unitOfWork,
                    userId);

                return Ok(new SessionInfo
                {
                    Email = email,
                    PersonalInfo = personalInfo,
                    UserId = userId
                });
            }
            catch (UserNotFoundException)
            {
                _log.LogError("Failed to find user for existing token. UserId: {UserId}", userId);
                return NotFound();
            }
        }
    }
}
