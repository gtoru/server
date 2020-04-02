using System;
using server.core.Domain.Misc;

namespace Domain.Tests.Helpers
{
    public class FakeTimeProvider : ITimeProvider
    {
        public DateTime CurrentTime { get; set; }

        public DateTime GetCurrent()
        {
            return CurrentTime;
        }
    }
}
