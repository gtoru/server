using System;
using server.core.Domain.Authentication;

namespace server.core.Domain
{
    public class User
    {
        public User()
        {
        }

        private User(Password password, Email email, PersonalInfo personalInfo)
        {
            Password = password;
            Email = email;
            PersonalInfo = personalInfo;
            UserId = Guid.NewGuid();
        }

        public User(Password password, Email email, Guid userId, PersonalInfo personalInfo)
        {
            Password = password;
            Email = email;
            UserId = userId;
            PersonalInfo = personalInfo;
        }

        public Password Password { get; }
        public Email Email { get; }
        public Guid UserId { get; }

        public PersonalInfo PersonalInfo { get; }

        public static User CreateNew(string email, string password, PersonalInfo personalInfo)
        {
            var hashedPassword = Password.Create(HashAlgorithm.BCrypt, password);
            var domainEmail = Email.Create(email);
            return new User(hashedPassword, domainEmail, personalInfo);
        }
    }
}
