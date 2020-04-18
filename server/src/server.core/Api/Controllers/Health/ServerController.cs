using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using server.core.Api.Infrastructure;

namespace server.core.Api.Controllers.Health
{
    [ApiController]
    [Route("_control")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ServerController : ControllerBase
    {
        private readonly ILogger<ServerController> _log;
        private readonly ShutdownManager _shutdownManager;

        public ServerController(ILogger<ServerController> log, ShutdownManager shutdownManager)
        {
            _log = log;
            _shutdownManager = shutdownManager;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("stop")]
        public IActionResult StartShutdown()
        {
            if (!IsLocalRequest(HttpContext.Request))
                return StatusCode(403);

            _log.LogWarning("Starting shutdown process");
            _shutdownManager.StartShutdown();

            return Ok();
        }

        private static bool IsLocalRequest(HttpRequest req)
        {
            var connection = req.HttpContext.Connection;
            if (connection.RemoteIpAddress != null)
                return connection.LocalIpAddress != null
                    ? connection.RemoteIpAddress.Equals(connection.LocalIpAddress)
                    : IPAddress.IsLoopback(connection.RemoteIpAddress);

            return connection.RemoteIpAddress == null && connection.LocalIpAddress == null;
        }
    }
}
