using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using server.core.Domain;
using server.core.Domain.Storage;
using server.core.Infrastructure.Error.AlreadyExists;
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
                .FirstOrDefaultAsync(u => u.Email.Address == email);

            if (user is null)
                throw new UserNotFoundException();

            await _dbContext.Entry(user)
                .Collection(u => u.TestSessions)
                .LoadAsync();

            await _dbContext.Entry(user)
                .Collection(u => u.TestSessions)
                .Query()
                .Select(t => t.Quiz)
                .LoadAsync();

            foreach (var quiz in user.TestSessions.Select(s => s.Quiz))
            {
                await _dbContext.Entry(quiz)
                    .Collection(q => q.Tasks)
                    .LoadAsync();

                foreach (var task in quiz.Tasks)
                    await _dbContext.Entry(task)
                        .Reference(t => t.Task)
                        .LoadAsync();
            }

            return user;
        }

        public async Task<User> FindUserAsync(Guid id)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user is null)
                throw new UserNotFoundException();

            await _dbContext.Entry(user)
                .Collection(u => u.TestSessions)
                .LoadAsync();

            await _dbContext.Entry(user)
                .Collection(u => u.TestSessions)
                .Query()
                .Select(t => t.Quiz)
                .LoadAsync();

            foreach (var quiz in user.TestSessions.Select(s => s.Quiz))
            {
                await _dbContext.Entry(quiz)
                    .Collection(q => q.Tasks)
                    .LoadAsync();

                foreach (var task in quiz.Tasks)
                    await _dbContext.Entry(task)
                        .Reference(t => t.Task)
                        .LoadAsync();
            }

            return user;
        }

        public async Task AddUserAsync(User user)
        {
            var foundUser = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Email.Address == user.Email.Address);

            if (foundUser != null)
                throw new UserAlreadyExistsException();

            await _dbContext.Users.AddAsync(user);
        }

        public async Task<int> GetUserCountAsync()
        {
            var userCount = await _dbContext.Users
                .CountAsync();

            return userCount;
        }
    }
}
