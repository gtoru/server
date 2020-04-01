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
        private readonly Func<UserContext> _dbFactory;

        public UserRepository(Func<UserContext> dbFactory)
        {
            _dbFactory = dbFactory;

            using var ctx = dbFactory();

            ctx.Database.EnsureCreated();
        }

        public async Task<User> FindUserAsync(Guid id)
        {
            await using var ctx = _dbFactory();

            var user = await ctx.Users.FindAsync(id);

            if (user is null)
                throw new UserNotFoundException();

            return UserModel.ToDomain(user);
        }

        public async Task AddUserAsync(User user)
        {
            await using var ctx = _dbFactory();

            var foundUser = await ctx.Users
                .SingleOrDefaultAsync(u => u.EmailAddress == user.Email.Address);

            if (foundUser != null)
                throw new UserAlreadyExistsException();

            await ctx.Users.AddAsync(UserModel.FromDomain(user));
            await ctx.SaveChangesAsync();
        }
    }
}
