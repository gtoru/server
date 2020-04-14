using System.Threading.Tasks;
using server.core.Domain.Storage;

namespace server.core.Infrastructure
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        IQuizRepository Quizzes { get; }
        ITestSessionRepository TestSessions { get; }
        ITaskRepository Tasks { get; }
        Task SaveAsync();
    }
}
