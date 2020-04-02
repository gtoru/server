using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using server.core.Domain;
using server.core.Domain.Storage;
using server.core.Infrastructure.Contexts;
using server.core.Infrastructure.Error;
using server.core.Infrastructure.Models;

namespace server.core.Infrastructure
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _dbContext;

        public UserRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User> FindUserAsync(Guid id)
        {
            var user = await _dbContext.Users.FindAsync(id);

            if (user is null)
                throw new UserNotFoundException();

            return UserModel.ToDomain(user);
        }

        public async Task AddUserAsync(User user)
        {
            var userModel = UserModel.FromDomain(user);

            var foundUser = await _dbContext.Users
                .SingleOrDefaultAsync(u => u.EmailAddress == userModel.EmailAddress);

            if (foundUser != null)
                throw new UserAlreadyExistsException();

            await _dbContext.Users.AddAsync(userModel);
        }
    }
}
