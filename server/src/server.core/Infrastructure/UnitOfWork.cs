using System.Threading.Tasks;
using server.core.Domain.Storage;
using server.core.Infrastructure.Storage;

namespace server.core.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;
        private IQuizRepository _quizzes;
        private ITaskRepository _tasks;
        private ITestSessionRepository _testSessions;
        private IUserRepository _users;

        public UnitOfWork(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IUserRepository Users => _users ??= new UserRepository(_dbContext);

        public IQuizRepository Quizzes => _quizzes ??= new QuizRepository(_dbContext);

        public ITestSessionRepository TestSessions => _testSessions ??= new TestSessionRepository(_dbContext);

        public ITaskRepository Tasks => _tasks ??= new TaskRepository(_dbContext);

        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
