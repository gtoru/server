using System.Threading;

namespace server.core.Api.Controllers.Health
{
    public class StatusReporter
    {
        private volatile int _state;

        public StatusReporter()
        {
            _state = 0;
        }

        public void SetReady()
        {
            Interlocked.Increment(ref _state);
        }

        public bool IsAlive()
        {
            return true;
        }

        public bool IsReady()
        {
            return _state == 1;
        }
    }
}
