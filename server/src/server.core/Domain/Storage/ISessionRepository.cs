using System;
using System.Threading.Tasks;
using server.core.Domain.Authentication;

namespace server.core.Domain.Storage
{
    public interface ISessionRepository
    {
        Task AddSessionAsync(Session session);
        Task<Session> FindSessionAsync(Guid sessionId);
    }
}
