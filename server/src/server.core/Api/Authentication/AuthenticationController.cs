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
