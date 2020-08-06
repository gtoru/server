using System;
using System.Collections.Generic;
using System.Linq;
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

        public static async Task<int> GetUserCountAsync(IUnitOfWork unitOfWork)
        {
            var userCount = await unitOfWork.Users.GetUserCountAsync();

            return userCount;
        }

        public static async Task<IEnumerable<(string, int)>> GetUsersWithFinishedSessions(IUnitOfWork unitOfWork)
        {
            return (await unitOfWork.Users.GetUsersByConditionAsync(u => u.HasFinishedSession()))
                .AsEnumerable()
                .Select(u => (u.Email.Address, u.GetRecentResult()));
        }
    }
}
