using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using server.core.Api.Dto;
using server.core.Application;
using server.core.Domain.Error;
using server.core.Domain.Tasks;
using server.core.Infrastructure;
using server.core.Infrastructure.Error.NotFound;
using Swashbuckle.AspNetCore.Annotations;
using AuthorizationPolicy = server.core.Api.Authorization.AuthorizationPolicy;

namespace server.core.Api.Controllers
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/user")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _log;

        public UserController(ILogger<UserController> log)
        {
            _log = log;
        }

        [HttpPost]
        [Route("{userId}/sessions/new")]
        [Authorize(AuthorizationPolicy.CanOnlyAccessOwnSessions)]
        public async Task<ActionResult<TestSessionResponse>> StartNewTestSessionAsync(
            [FromServices] IUnitOfWork unitOfWork,
            [FromRoute] Guid userId,
            [FromBody] NewTestSessionRequest request)
        {
            try
            {
                var newSession =
                    await TestSessionManager.StartTestSessionAsync(unitOfWork, userId, request.QuizId);
                return Ok(SessionToResponse(newSession));
            }
            catch (UserNotFoundException)
            {
                _log.LogWarning("user {UserId} not found", userId);
                return NotFound("user not found");
            }
            catch (QuizNotFoundException)
            {
                _log.LogWarning("quiz {QuizId} not found", request.QuizId);
                return NotFound("quiz not found");
            }
            catch (AlreadyHasActiveSessionException)
            {
                _log.LogWarning("user {UserId} already has active session", userId);
                return Conflict("already has active session");
            }
        }

        [HttpGet]
        [Route("{userId}/sessions/current")]
        [Authorize(AuthorizationPolicy.CanOnlyAccessOwnSessions)]
        public async Task<ActionResult<TestSessionResponse>> GetCurrentSession(
            [FromServices] IUnitOfWork unitOfWork,
            [FromRoute] Guid userId)
        {
            try
            {
                var currentSession = await
                    TestSessionManager.GetActiveSession(unitOfWork, userId);

                return Ok(SessionToResponse(currentSession));
            }
            catch (NoActiveSessionsException)
            {
                _log.LogWarning("no active sessions found for {UserId}", userId);
                return NotFound("no active sessions");
            }
            catch (UserNotFoundException)
            {
                _log.LogWarning("user {UserId} not found", userId);
                return NotFound("user not found");
            }
        }

        [HttpGet]
        [Authorize(AuthorizationPolicy.CanOnlyAccessOwnSessions)]
        [Route("{userId}/sessions/results")]
        public async Task<ActionResult<GetResultsResponse>> GetResultsAsync(
            [FromServices] IUnitOfWork unitOfWork,
            [FromRoute] Guid userId)
        {
            try
            {
                var results =
                    await TestSessionManager.GetResultsAsync(unitOfWork, userId);

                return Ok(new GetResultsResponse
                {
                    Results = results.Select(r => new GetResultResponse
                    {
                        Result = r.result,
                        TestSessionId = r.sessionId
                    }).ToList()
                });
            }
            catch (UserNotFoundException)
            {
                _log.LogWarning("user {UserId} not found");
                return NotFound("user not found");
            }
        }

        [HttpPost]
        [Authorize(AuthorizationPolicy.CanOnlyAccessOwnSessions)]
        [Route("{userId}/sessions/end")]
        public async Task<ActionResult<GetResultResponse>> EndSessionAsync(
            [FromServices] IUnitOfWork unitOfWork,
            [FromRoute] Guid userId)
        {
            try
            {
                var (sessionId, result) =
                    await TestSessionManager.EndCurrentSessionAsync(unitOfWork, userId);

                return Ok(new GetResultResponse
                {
                    Result = result,
                    TestSessionId = sessionId
                });
            }
            catch (NoActiveSessionsException)
            {
                _log.LogInformation("user {UserId} has no active sessions", userId);
                return NotFound("session not found");
            }
            catch (UserNotFoundException)
            {
                _log.LogWarning("user {UserId} not found", userId);
                return NotFound("user not found");
            }
        }

        [HttpPost]
        [Authorize(AuthorizationPolicy.CanOnlyAccessOwnSessions)]
        [Route("{userId}/sessions/answer")]
        public async Task<IActionResult> AddAnswersAsync(
            [FromServices] IUnitOfWork unitOfWork,
            [FromRoute] Guid userId,
            [FromBody] AddAnswersRequest request)
        {
            try
            {
                await TestSessionManager.AddAnswersAsync(unitOfWork, userId,
                    request.Answers.Select(a => (a.TaskNumber, a.Answer)));
                return Ok();
            }
            catch (UserNotFoundException)
            {
                _log.LogWarning("user {UserId} not found");
                return NotFound();
            }
        }

        [HttpGet]
        [Authorize(AuthorizationPolicy.OnlyAdmins)]
        [Route("count")]
        [SwaggerOperation(
            Description = "Needs admin rights",
            Summary = "Returns current total user count")]
        [SwaggerResponse(200, "User count returned", typeof(UserCountResponse))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Not an admin")]
        public async Task<ActionResult<UserCountResponse>> GetUserCountAsync(
            [FromServices] IUnitOfWork unitOfWork)
        {
            var userCount = await UserManager.GetUserCountAsync(unitOfWork);

            return Ok(new UserCountResponse
            {
                UserCount = userCount
            });
        }

        private static TestSessionResponse SessionToResponse(TestSession session)
        {
            return new TestSessionResponse
            {
                TestSessionId = session.SessionId,
                Tasks = session.Quiz.Tasks.Select(t =>
                    new TaskResponse
                    {
                        Answers = t.Task.Variants,
                        Question = t.Task.Question
                    }).ToList()
            };
        }
    }
}
