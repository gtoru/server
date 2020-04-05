using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using server.core.Api.Dto;
using server.core.Application;
using server.core.Domain.Authentication;
using server.core.Domain.Error;
using server.core.Infrastructure;
using server.core.Infrastructure.Error;

namespace server.core.Api.Authentication
{
    [ApiController]
    [Route("api/auth/v1")]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _log;

        public AuthenticationController(ILogger<AuthenticationController> log)
        {
            _log = log;
        }

        [HttpPost("user/register")]
        public async Task<ActionResult> RegisterAsync([FromBody] RegistrationRequest request, IUnitOfWork unitOfWork)
        {
            try
            {
                await AuthenticationManager.RegisterUserAsync(unitOfWork, request.Email, request.Password, request.ExtractPersonalInfo());
            }
            catch (UserAlreadyExistsException)
            {
                _log.LogWarning("Trying to register user with existing email: {Email}", request.Email);
                return Conflict("user with specified email already exists");
            }

            return Ok();
        }

        [HttpPost("user/authenticate")]
        public async Task<ActionResult<string>> AuthenticateAsync([FromBody] AuthenticationRequest request,
            IUnitOfWork unitOfWork)
        {
            using var scope = _log.BeginScope("Begin user authentication: {Email}", request.Email);
            Session session;
            try
            {
                session = await AuthenticationManager.AuthenticateAsync(unitOfWork, request.Email, request.Password);
            }
            catch (UserNotFoundException)
            {
                _log.LogWarning("User with {Email} not found", request.Email);
                return Forbid();
            }
            catch (IncorrectPasswordException)
            {
                _log.LogWarning("Incorrect password provided");
                return Forbid();
            }

            return Ok(session.SessionId);
        }
    }
}
