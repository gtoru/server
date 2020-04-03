using System;
using server.core.Domain.Authentication;

namespace server.core.Domain
{
    public class User
    {
        public User()
        {
        }

        private User(Password password, Email email)
        {
            Password = password;
            Email = email;
            UserId = Guid.NewGuid();
        }

        public User(Password password, Email email, Guid userId)
        {
            Password = password;
            Email = email;
            UserId = userId;
        }

        public Password Password { get; }
        public Email Email { get; }
        public Guid UserId { get; }

        public static User CreateNew(string email, string password)
        {
            var hashedPassword = Password.Create(HashAlgorithm.BCrypt, password);
            var domainEmail = Email.Create(email);
            return new User(hashedPassword, domainEmail);
        }
    }
}
