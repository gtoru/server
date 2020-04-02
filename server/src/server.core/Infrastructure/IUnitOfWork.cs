using System.Threading.Tasks;
using server.core.Domain.Storage;

namespace server.core.Infrastructure
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }

        ISessionRepository Sessions { get; }

        Task SaveAsync();
    }
}
