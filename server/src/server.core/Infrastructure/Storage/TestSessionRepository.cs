using System;
using System.Threading.Tasks;
using server.core.Domain.Storage;
using server.core.Domain.Tasks;
using server.core.Infrastructure.Error.AlreadyExists;
using server.core.Infrastructure.Error.NotFound;

namespace server.core.Infrastructure.Storage
{
    public class TestSessionRepository : ITestSessionRepository
    {
        private readonly AppDbContext _dbContext;

        public TestSessionRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddTestSessionAsync(TestSession testSession)
        {
            var foundSession = await _dbContext.TestSessions.FindAsync(testSession.SessionId);

            if (foundSession != null)
                throw new SessionAlreadyExistsException();

            await _dbContext.TestSessions.AddAsync(testSession);
        }

        public async Task<TestSession> FindTestSessionAsync(Guid sessionId)
        {
            var foundSession = await _dbContext.TestSessions.FindAsync(sessionId);

            if (foundSession == null)
                throw new SessionNotFoundException();

            return foundSession;
        }
    }
}
