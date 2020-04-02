using System;

namespace server.core.Domain.Misc
{
    public interface ITimeProvider
    {
        DateTime GetCurrent();
    }
}
