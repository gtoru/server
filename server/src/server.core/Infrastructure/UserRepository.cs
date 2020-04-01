using System;
using System.Threading.Tasks;
using server.core.Domain;
using server.core.Domain.Storage;
using server.core.Infrastructure.Contexts;
using server.core.Infrastructure.Models;

namespace server.core.Infrastructure
{
    public class UserRepository : IUserRepository
    {
        private readonly Func<UserContext> _dbFactory;

        public UserRepository(Func<UserContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<User> FindUserAsync(Guid id)
        {
            await using var ctx = _dbFactory();

            var user = await ctx.Users.FindAsync(id);
            return UserModel.ToDomain(user);
        }

        public async Task AddUserAsync(User user)
        {
            await using var ctx = _dbFactory();

            await ctx.Users.AddAsync(UserModel.FromDomain(user));
            await ctx.SaveChangesAsync();
        }
    }
}
