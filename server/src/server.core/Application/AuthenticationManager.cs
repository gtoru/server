using System;
using System.Threading.Tasks;
using server.core.Domain;
using server.core.Domain.Authentication;
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

        public static async Task<Session> AuthenticateAsync(IUnitOfWork unitOfWork, string email, string password)
        {
            var user = await unitOfWork.Users.FindUserAsync(email);

            user.Password.Verify(password);

            Session session;
            try
            {
                session = await unitOfWork.Sessions.FindSessionByUserAsync(user.UserId);
            }
            catch (SessionNotFoundException)
            {
                session = Session.CreateNew(user.UserId);
                await unitOfWork.Sessions.AddSessionAsync(session);
            }

            return session;
        }

        public static async Task CheckSessionAsync(IUnitOfWork unitOfWork, Guid sessionId)
        {
            var session = await unitOfWork.Sessions.FindSessionAsync(sessionId);

            session.CheckSession();

            session.Prolongate();
        }
    }
}
