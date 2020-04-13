using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
    [Route("api/v{version:apiVersion}/task")]
    public class TasksController : ControllerBase
    {
        private readonly ILogger<TasksController> _log;

        public TasksController(ILogger<TasksController> log)
        {
            _log = log;
        }

        [HttpGet]
        [Route("all")]
        [Authorize(AuthorizationPolicy.OnlyAdmins)]
        [SwaggerOperation(
            Description = "Admin access required",
            Summary = "Gets all available tasks")]
        [SwaggerResponse(200, "All existing tasks", typeof(AllTasksResponse))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Not enough access rights")]
        public async Task<ActionResult<AllTasksResponse>> GetAll([FromServices] IUnitOfWork unitOfWork)
        {
            var result = new AllTasksResponse
            {
                Tasks = (await TaskManager.GetAllTasksAsync(unitOfWork))
                    .Select(t => new GetTaskResponse
                    {
                        Question = t.Question,
                        Answer = t.Answer,
                        Variants = t.Variants,
                        TaskId = t.TaskId
                    }).ToList()
            };

            return Ok(result);
        }

        [HttpGet]
        [Route("{taskId}")]
        [Authorize(AuthorizationPolicy.OnlyAdmins)]
        [SwaggerOperation(
            Description = "Requires admin access",
            Summary = "Returns task")]
        [SwaggerResponse(200, "Task", typeof(GetTaskResponse))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Not enough access rights")]
        [SwaggerResponse(404, "Task with specified ID not found")]
        public async Task<ActionResult<GetTaskResponse>> GetTask(
            [FromServices] IUnitOfWork unitOfWork,
            [FromRoute] Guid taskId)
        {
            try
            {
                var task = await TaskManager.GetTaskAsync(unitOfWork, taskId);
                return Ok(new GetTaskResponse
                {
                    Question = task.Question,
                    Answer = task.Answer,
                    Variants = task.Variants,
                    TaskId = task.TaskId
                });
            }
            catch (TaskNotFoundException)
            {
                return NotFound("task with specified ID not found");
            }
        }

        [HttpPost]
        [Route("new")]
        [Authorize(AuthorizationPolicy.OnlyAdmins)]
        [SwaggerOperation(
            Description = "Admin access required",
            Summary = "Creates new task")]
        [SwaggerResponse(200, "Task created", typeof(CreateTaskResponse))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Not enough access rights")]
        public async Task<ActionResult<CreateTaskResponse>> CreateNewTask(
            [FromServices] IUnitOfWork unitOfWork,
            [FromBody] CreateTaskRequest request)
        {
            var task = await TaskManager.AddTaskAsync(
                unitOfWork,
                request.Question,
                request.Answer,
                request.Variants);

            return Ok(new CreateTaskResponse
            {
                TaskId = task.TaskId
            });
        }
    }
}
