using Microsoft.AspNetCore.Mvc;

namespace server.core.Api.Controllers.Health
{
    [ApiController]
    [Route("_status")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HealthController : ControllerBase
    {
        private readonly StatusReporter _statusReporter;

        public HealthController(StatusReporter statusReporter)
        {
            _statusReporter = statusReporter;
        }

        [HttpGet("alive")]
        public IActionResult GetAlive()
        {
            return StatusCode(200);
        }

        [HttpGet("ready")]
        public IActionResult GetReady()
        {
            if (_statusReporter.Current != StatusReporter.Status.Ready)
                return StatusCode(503);
            return StatusCode(200);
        }
    }
}
