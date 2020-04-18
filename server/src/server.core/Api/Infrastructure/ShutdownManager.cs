using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace server.core.Api.Infrastructure
{
    public class ShutdownManager
    {
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly TimeSpan _nextShutdownAttemptDelay = TimeSpan.FromMilliseconds(500);
        private readonly StatusReporter _statusReporter;

        public ShutdownManager(
            StatusReporter statusReporter,
            IHostApplicationLifetime hostApplicationLifetime)
        {
            _statusReporter = statusReporter;
            _hostApplicationLifetime = hostApplicationLifetime;
        }

        public bool InProgress => _statusReporter.IsShuttingDown();

        public async Task ShutdownAsync()
        {
            // makes readiness probe fail and stops request routing
            _statusReporter.SetShutdown();

            while (true)
            {
                // which means that shutdown request is the only active request
                if (_statusReporter.ActiveUnits <= 1)
                    break;

                await Task.Delay(_nextShutdownAttemptDelay);
            }

            _hostApplicationLifetime.StopApplication();
        }
    }
}
