using System;
using server.core.Domain.Error;

namespace server.core.Domain.Authentication
{
    public class Session
    {
        private static readonly TimeSpan ExpirationTime = TimeSpan.FromDays(30);
        private static readonly TimeSpan ProlongationPeriod = TimeSpan.FromDays(10);

        private Session(Guid userId)
        {
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

        public void CheckSession()
        {
            if (DateTime.UtcNow > ValidThrough)
                throw new SessionExpiredException();
        }

        public void Prolongate()
        {
            ValidThrough += ProlongationPeriod;
        }
    }
}
