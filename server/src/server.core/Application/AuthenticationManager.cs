using System.Threading.Tasks;
using server.core.Domain;
using server.core.Infrastructure;
using server.core.Infrastructure.Error;

namespace server.core.Application
{
    public static class AuthenticationManager
    {
        public static async Task<User> RegisterUserAsync(IUnitOfWork unitOfWork, string email, string password,
            PersonalInfo personalInfo)
        {
            var user = User.CreateNew(email, password, personalInfo);

            await unitOfWork.Users.AddUserAsync(user);

            return user;
        }

        public static async Task CreateAdmin(IUnitOfWork unitOfWork, string login, string password)
        {
            var admin = User.CreateAdmin(login, password);

            try
            {
                await unitOfWork.Users.AddUserAsync(admin);
            }
            catch (UserAlreadyExistsException)
            {
            }
        }

        public static async Task<User> AuthenticateAsync(IUnitOfWork unitOfWork, string email, string password)
        {
            var user = await unitOfWork.Users.FindUserAsync(email);

            user.Password.Verify(password);

            return user;
        }
    }
}
