namespace server.core.Api.Controllers.Health
{
    public class StatusReporter
    {
        public enum Status
        {
            Alive,
            Ready
        }

        private readonly object sync = new object();

        private Status _current;

        public StatusReporter()
        {
            Current = Status.Alive;
        }

        public Status Current
        {
            get
            {
                lock (sync)
                {
                    return _current;
                }
            }
            set
            {
                lock (sync)
                {
                    _current = value;
                }
            }
        }
    }
}
