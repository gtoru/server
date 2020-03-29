using System;

namespace server.core.Domain.Storage
{
    public interface IUserRepository
    {
        User FindUser(Guid id);
        void AddUser(User user);
    }
}
