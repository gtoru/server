using System;
using System.Collections.Generic;
using System.Linq;
using server.core.Domain.Authentication;
using server.core.Domain.Error;
using server.core.Domain.Tasks;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

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

        public Password Password { get; private set; }
        public Email Email { get; private set; }
        public Guid UserId { get; private set; }
        public PersonalInfo PersonalInfo { get; private set; }
        public AccessLevel AccessLevel { get; private set; }
        public List<TestSession> TestSessions { get; private set; }

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

        public TestSession GetActiveSession()
        {
            if (!HasActiveSession())
                throw new NoActiveSessionsException();

            return CurrentSession;
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

        public int GetRecentResult()
        {
            var lastSession = TestSessions.LastOrDefault();

            if (HasActiveSession() && TestSessions.Count >= 2)
                lastSession = TestSessions[^2];

            if (lastSession == null)
                throw new NoFinishedSessionsException();

            return lastSession.Result;
        }

        public bool HasFinishedSession()
        {
            return HasActiveSession() && TestSessions.Count >= 2 ||
                   !HasActiveSession() && TestSessions.Count >= 1;
        }
    }
}
