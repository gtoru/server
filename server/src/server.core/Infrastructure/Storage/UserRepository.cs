using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using server.core.Domain;
using server.core.Domain.Storage;
using server.core.Infrastructure.Error.NotFound;

namespace server.core.Infrastructure.Storage
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _dbContext;

        public UserRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User> FindUserAsync(string email)
        {
            var user = await _dbContext.Users
                .Include(u => u.TestSessions)
                .FirstOrDefaultAsync(u => u.Email.Address == email);

            if (user is null)
                throw new UserNotFoundException();

            return user;
        }

        public async Task<User> FindUserAsync(Guid id)
        {
            var user = await _dbContext.Users
                .Include(u => u.TestSessions)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user is null)
                throw new UserNotFoundException();

            return user;
        }

        public async Task AddUserAsync(User user)
        {
            await _dbContext.Users.AddAsync(user);
        }
    }
}
