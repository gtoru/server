using Microsoft.AspNetCore.Mvc;
using server.core.Api.Infrastructure;

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
            if (_statusReporter.IsAlive())
                return StatusCode(200);
            return StatusCode(503);
        }

        [HttpGet("ready")]
        public IActionResult GetReady()
        {
            if (_statusReporter.IsReady())
                return StatusCode(200);
            return StatusCode(503);
        }
    }
}
