using System;
using System.Threading.Tasks;
using server.core.Domain.Tasks;

namespace server.core.Domain.Storage
{
    public interface ITestSessionRepository
    {
        Task AddTestSessionAsync(TestSession testSession);
        Task<TestSession> FindTestSessionAsync(Guid sessionId);
    }
}
