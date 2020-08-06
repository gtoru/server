using System;
using System.Linq;
using System.Threading.Tasks;

namespace server.core.Domain.Storage
{
    public interface IUserRepository
    {
        Task<User> FindUserAsync(string email);
        Task<User> FindUserAsync(Guid id);
        Task AddUserAsync(User user);
        Task<int> GetUserCountAsync();
        Task<IQueryable<User>> GetUsersByConditionAsync(Func<User, bool> condition);
    }
}
