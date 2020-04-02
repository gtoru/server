using System;

namespace server.core.Domain.Misc
{
    public class UtcTimeProvider : ITimeProvider
    {
        public DateTime GetCurrent()
        {
            return DateTime.UtcNow;
        }
    }
}
