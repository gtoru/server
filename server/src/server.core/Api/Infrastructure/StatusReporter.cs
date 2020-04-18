using System.Threading;

namespace server.core.Api.Infrastructure
{
    public class StatusReporter
    {
        private volatile int _activeUnits;
        private volatile int _state;

        public StatusReporter()
        {
            _state = 0;
        }

        public int ActiveUnits => _activeUnits;

        public void SetReady()
        {
            Interlocked.Exchange(ref _state, 1);
        }

        public void SetShutdown()
        {
            Interlocked.Exchange(ref _state, 2);
        }

        public void AddActiveUnit()
        {
            Interlocked.Increment(ref _activeUnits);
        }

        public void RemoveActiveUnt()
        {
            Interlocked.Decrement(ref _activeUnits);
        }

        public bool IsAlive()
        {
            return true;
        }

        public bool IsReady()
        {
            return _state == 1;
        }

        public bool IsShuttingDown()
        {
            return _state == 2;
        }
    }
}
