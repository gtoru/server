using System.Threading.Tasks;
using server.core.Infrastructure;

namespace server.core.Api.Authentication
{
    public interface IAuthenticator
    {
        Task<string> AuthenticateAsync(IUnitOfWork unitOfWork, string email, string password);
    }
}
