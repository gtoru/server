using System;
using System.Threading.Tasks;
using server.core.Domain;
using server.core.Infrastructure;

namespace server.core.Application
{
    public static class UserManager
    {
        public static async Task<(string, PersonalInfo)> GetUserInfoAsync(
            IUnitOfWork unitOfWork,
            Guid userId)
        {
            var user = await unitOfWork.Users.FindUserAsync(userId);

            return (user.Email.Address, user.PersonalInfo);
        }
    }
}
