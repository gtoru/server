using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using server.core.Domain.Authentication;
using server.core.Domain.Storage;
using server.core.Infrastructure.Error;

namespace server.core.Infrastructure.Storage
{
    public class SessionRepository : ISessionRepository
    {
        private readonly AppDbContext _dbContext;

        public SessionRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddSessionAsync(Session session)
        {
            var foundSession = await _dbContext.Sessions
                .SingleOrDefaultAsync(m =>
                    m.UserId == session.UserId);

            if (foundSession != null)
                throw new SessionAlreadyExistsException();

            await _dbContext.Sessions.AddAsync(session);
        }

        public async Task<Session> FindSessionAsync(Guid sessionId)
        {
            var foundSession =
                await _dbContext.Sessions.FindAsync(sessionId);

            if (foundSession is null)
                throw new SessionNotFoundException();

            return foundSession;
        }

        public async Task<Session> FindSessionByUserAsync(Guid userId)
        {
            var foundSession =
                await _dbContext.Sessions.SingleOrDefaultAsync(s => s.UserId == userId);

            if (foundSession == null)
                throw new SessionNotFoundException();

            return foundSession;
        }

        public async Task PatchSessionAsync(Guid sessionId, Action<Session> patch)
        {
            var foundSession = await _dbContext.Sessions.FindAsync(sessionId);

            if (foundSession is null)
                throw new SessionNotFoundException();

            patch(foundSession);
        }
    }
}
