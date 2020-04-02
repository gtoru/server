using System.Threading.Tasks;
using server.core.Domain.Storage;
using server.core.Infrastructure.Storage;

namespace server.core.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;
        private ISessionRepository _sessions;
        private IUserRepository _users;

        public UnitOfWork(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            dbContext.Database.EnsureCreated();
        }

        public IUserRepository Users
        {
            get
            {
                if (_users is null)
                    _users = new UserRepository(_dbContext);
                return _users;
            }
        }

        public ISessionRepository Sessions
        {
            get
            {
                if (_sessions is null)
                    _sessions = new SessionRepository(_dbContext);
                return _sessions;
            }
        }

        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
