using System;
using System.ComponentModel.DataAnnotations;
using server.core.Domain.Authentication;

namespace server.core.Infrastructure.Models
{
    public class SessionModel
    {
        [Key]
        [Required]
        public Guid SessionId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public DateTime ValidThrough { get; set; }

        public static Session ToDomain(SessionModel sessionModel)
        {
            var session = new Session(
                sessionModel.UserId,
                sessionModel.SessionId,
                sessionModel.ValidThrough);
            return session;
        }

        public static SessionModel FromDomain(Session session)
        {
            return new SessionModel
            {
                SessionId = session.Id,
                UserId = session.UserId,
                ValidThrough = session.ValidThrough
            };
        }
    }
}
