using System;
using System.Collections.Generic;
using System.Linq;
using server.core.Domain.Authentication;
using server.core.Domain.Error;
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
        }

        public User(Password password, Email email, Guid userId, PersonalInfo personalInfo, AccessLevel accessLevel)
        {
            Password = password;
            Email = email;
            UserId = userId;
            PersonalInfo = personalInfo;
            AccessLevel = accessLevel;
            TestSessions = new List<TestSession>();
        }

        public Password Password { get; }
        public Email Email { get; }
        public Guid UserId { get; }
        public PersonalInfo PersonalInfo { get; }
        public AccessLevel AccessLevel { get; private set; }
        public List<TestSession> TestSessions { get; set; }
        public TestSession CurrentSession
        {
            get
            {
                if (TestSessions.Count == 0)
                    throw new NoSessionsException();
                return TestSessions.Last();
            }
        }
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

        public bool HasActiveSession()
        {
            if (TestSessions.Count == 0)
                return false;

            return !CurrentSession.IsFinished && !CurrentSession.Expired();
        }

        public void StartNewSession(Quiz quiz)
        {
            if (HasActiveSession())
                throw new AlreadyHasActiveSessionException();

            var session = TestSession.CreateNew(
                this,
                quiz);

            TestSessions.Add(session);
        }
    }
}
