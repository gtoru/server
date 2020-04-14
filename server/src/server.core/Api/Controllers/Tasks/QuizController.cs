using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.core.Api.Dto;
using server.core.Application;
using server.core.Infrastructure;
using server.core.Infrastructure.Error.NotFound;
using Swashbuckle.AspNetCore.Annotations;
using AuthorizationPolicy = server.core.Api.Authorization.AuthorizationPolicy;

namespace server.core.Api.Controllers.Tasks
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/quiz")]
    public class QuizController : ControllerBase
    {
        [HttpPost]
        [Route("new")]
        [Authorize(AuthorizationPolicy.OnlyAdmins)]
        [SwaggerOperation(
            Description = "Needs admin rights",
            Summary = "Creates new quiz")]
        [SwaggerResponse(200, "Quiz created", typeof(CreateQuizResponse))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Not enough access rights")]
        [SwaggerResponse(404, "One or more tasks not found")]
        public async Task<ActionResult<CreateQuizResponse>> CreateNewQuiz(
            [FromServices] IUnitOfWork unitOfWork,
            [FromBody] CreateQuizRequest request)
        {
            try
            {
                var createdQuiz = await TaskManager.AddQuizAsync(unitOfWork, request.Tasks);
                return Ok(new CreateQuizResponse
                {
                    QuizId = createdQuiz.QuizId
                });
            }
            catch (TaskNotFoundException)
            {
                return NotFound("one or more tasks not found");
            }
        }

        [HttpGet]
        [Route("{quizId}")]
        [Authorize(AuthorizationPolicy.OnlyAdmins)]
        [SwaggerOperation(
            Description = "Requires admin rights",
            Summary = "Returns quiz")]
        [SwaggerResponse(200, "Found quiz", typeof(GetQuizResponse))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Not enough access rights")]
        [SwaggerResponse(404, "Quiz not found")]
        public async Task<ActionResult<GetQuizResponse>> GetQuiz(
            [FromServices] IUnitOfWork unitOfWork,
            [FromRoute] Guid quizId)
        {
            try
            {
                var foundQuiz = await TaskManager.GetQuizAsync(unitOfWork, quizId);
                var result = new GetQuizResponse
                {
                    QuizId = foundQuiz.QuizId,
                    Tasks = foundQuiz.Tasks.Select(
                        t => new GetTaskResponse
                        {
                            TaskId = t.TaskId,
                            Question = t.Task.Question,
                            Variants = t.Task.Variants,
                            Answer = t.Task.Answer
                        }).ToList()
                };
                return Ok(result);
            }
            catch (QuizNotFoundException)
            {
                return NotFound("Quiz not found");
            }
        }

        [HttpGet]
        [Route("all")]
        [SwaggerOperation(
            Description = "Only retrieves quiz info",
            Summary = "Returns all available quizzes")]
        [SwaggerResponse(200, "All quizzes", typeof(AllQuizzesResponse))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Not enough access rights")]
        public async Task<ActionResult<AllQuizzesResponse>> GetAllQuizzes(
            [FromServices] IUnitOfWork unitOfWork)
        {
            var quizzes = await TaskManager.GetAllQuizzesAsync(unitOfWork);

            var result = quizzes.Select(q => new QuizInfo
            {
                QuizId = q.QuizId
            });

            return Ok(new AllQuizzesResponse
            {
                Quizzes = result.ToList()
            });
        }
    }
}
