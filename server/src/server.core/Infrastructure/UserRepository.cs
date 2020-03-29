using System;
using server.core.Domain;
using server.core.Domain.Storage;

namespace server.core.Infrastructure
{
    public class UserRepository : IUserRepository
    {
        public User FindUser(Guid id)
        {
            throw new NotImplementedException();
        }

        public void AddUser(User user)
        {
            throw new NotImplementedException();
        }
    }
}
