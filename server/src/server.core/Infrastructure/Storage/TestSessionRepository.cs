using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using server.core.Domain.Storage;
using server.core.Domain.Tasks;
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
            await _dbContext.TestSessions.AddAsync(testSession);
        }

        public async Task<TestSession> FindTestSessionAsync(Guid sessionId)
        {
            var foundSession = await _dbContext.TestSessions
                .FirstOrDefaultAsync(s => s.SessionId == sessionId);

            if (foundSession == null)
                throw new SessionNotFoundException();

            return foundSession;
        }
    }
}
