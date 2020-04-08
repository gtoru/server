using System;
using System.Collections.Generic;
using server.core.Domain.Authentication;
using server.core.Domain.Tasks;

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
            AccessLevel = AccessLevel.User;
            TestSessions = new List<TestSession>();
            CurrentSession = Guid.Empty;
        }

        public User(Password password, Email email, Guid userId, PersonalInfo personalInfo, AccessLevel accessLevel)
        {
            Password = password;
            Email = email;
            UserId = userId;
            PersonalInfo = personalInfo;
            AccessLevel = accessLevel;
            TestSessions = new List<TestSession>();
            CurrentSession = Guid.Empty;
        }

        public Password Password { get; }
        public Email Email { get; }
        public Guid UserId { get; }
        public PersonalInfo PersonalInfo { get; }
        public AccessLevel AccessLevel { get; private set; }
        public List<TestSession> TestSessions { get; set; }

        public Guid CurrentSession { get; set; }

        public static User CreateNew(string email, string password, PersonalInfo personalInfo)
        {
            var hashedPassword = Password.Create(HashAlgorithm.BCrypt, password);
            var domainEmail = Email.Create(email);
            return new User(hashedPassword, domainEmail, personalInfo);
        }

        public static User CreateAdmin(string email, string password)
        {
            var hashedPassword = Password.Create(HashAlgorithm.BCrypt, password);
            var domainEmail = Email.Create(email);
            var emptyInfo = new PersonalInfo("", DateTime.UtcNow, "", "", "");
            var user = new User(hashedPassword, domainEmail, emptyInfo)
            {
                AccessLevel = AccessLevel.Administrator
            };
            return user;
        }
    }
}
