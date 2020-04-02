using System;
using System.Runtime.CompilerServices;
using server.core.Domain.Error;
using server.core.Domain.Misc;

[assembly: InternalsVisibleTo("Domain.Tests")]

namespace server.core.Domain.Authentication
{
    public class Session
    {
        private static readonly TimeSpan ExpirationTime = TimeSpan.FromDays(30);
        private static readonly TimeSpan ProlongationPeriod = TimeSpan.FromDays(10);
        private readonly ITimeProvider _timeProvider;

        private Session(Guid userId, ITimeProvider timeProvider = null)
        {
            _timeProvider = timeProvider ?? new UtcTimeProvider();
            Id = Guid.NewGuid();
            ValidThrough = DateTime.UtcNow + ExpirationTime;
            UserId = userId;
        }

        public DateTime ValidThrough { get; private set; }
        public Guid Id { get; }
        public Guid UserId { get; }

        public static Session CreateNew(Guid userId)
        {
            return new Session(userId);
        }

        internal static Session CreateNew(Guid userId, ITimeProvider timeProvider)
        {
            return new Session(userId, timeProvider);
        }

        public void CheckSession()
        {
            if (_timeProvider.GetCurrent() > ValidThrough)
                throw new SessionExpiredException();
        }

        public void Prolongate()
        {
            ValidThrough += ProlongationPeriod;
        }
    }
}
