using System;
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
                .SingleOrDefaultAsync(u => u.Email.Address == email);

            if (user is null)
                throw new UserNotFoundException();

            return user;
        }

        public async Task<User> FindUserAsync(Guid id)
        {
            var user = await _dbContext.Users.FindAsync(id);

            if (user is null)
                throw new UserNotFoundException();

            return user;
        }

        public async Task AddUserAsync(User user)
        {
            var foundUser = await _dbContext.Users
                .SingleOrDefaultAsync(u => u.Email.Address == user.Email.Address);

            if (foundUser != null)
                throw new UserAlreadyExistsException();

            await _dbContext.Users.AddAsync(user);
        }
    }
}
