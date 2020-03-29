using System;
using server.core.Domain;
using server.core.Domain.Authentication;
using server.core.Domain.Error;

namespace server.core.Infrastructure.Dto
{
    public class UserDto
    {
        public Guid UserId { get; set; }
        public bool IsVerified { get; set; }
        public string EmailAddress { get; set; }
        public string HashAlgorithm { get; set; }
        public string PasswordHash { get; set; }

        public static UserDto FromDomain(User user)
        {
            return new UserDto
            {
                UserId = user.UserId,
                EmailAddress = user.Email.Address,
                IsVerified = user.Email.IsVerified,
                PasswordHash = user.Password.HashedPassword,
                HashAlgorithm = user.Password.HashAlgorithm.ToString()
            };
        }

        public static User ToDomain(UserDto user)
        {
            if (!Enum.TryParse<HashAlgorithm>(user.HashAlgorithm, out var hashAlgorithm)
                || !Enum.IsDefined(typeof(HashAlgorithm), hashAlgorithm))
            {
                throw new UnknownHashAlgorithmException();
            }

            return new User(new Password(hashAlgorithm, user.PasswordHash),
                new Email(user.EmailAddress, user.IsVerified), user.UserId);
        }
    }
}
