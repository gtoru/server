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

        public void StartShutdown()
        {
            _statusReporter.SetShutdown();
            Task.Run(ShutdownAsync);
        }

        private async Task ShutdownAsync()
        {
            while (true)
            {
                if (_statusReporter.ActiveUnits == 0)
                    break;

                await Task.Delay(_nextShutdownAttemptDelay);
            }

            _hostApplicationLifetime.StopApplication();
        }
    }
}
