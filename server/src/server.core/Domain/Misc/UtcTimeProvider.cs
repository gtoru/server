using System;

namespace server.core.Domain.Misc
{
    public class UtcTimeProvider : ITimeProvider
    {
        public static readonly UtcTimeProvider Instance = new UtcTimeProvider();

        public DateTime GetCurrent()
        {
            return DateTime.UtcNow;
        }
    }
}
