using System;
using server.core.Domain.Error;

namespace server.core.Domain.Authentication
{
    public class Session
    {
        private static readonly TimeSpan ExpirationTime = TimeSpan.FromDays(30);
        private Session(Guid userId)
        {
            Id = Guid.NewGuid();
            CreationTime = DateTime.UtcNow;
            UserId = userId;
        }

        public DateTime CreationTime { get; }
        public Guid Id { get; }
        public Guid UserId { get; }

        public static Session CreateNew(Guid userId)
        {
            return new Session(userId);
        }

        public void CheckSession()
        {
            if (DateTime.UtcNow - CreationTime > ExpirationTime)
                throw new SessionExpiredException();
        }
    }
}
