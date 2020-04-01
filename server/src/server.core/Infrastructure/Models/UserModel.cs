using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using server.core.Domain;
using server.core.Domain.Authentication;
using server.core.Domain.Error;

namespace server.core.Infrastructure.Models
{
    public class UserModel
    {
        [Key]
        public Guid UserId { get; set; }

        [Required, DefaultValue(false)]
        public bool IsVerified { get; set; }

        [Required]
        public string EmailAddress { get; set; }

        [Required]
        public string HashAlgorithm { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public static UserModel FromDomain(User user)
        {
            return new UserModel
            {
                UserId = user.UserId,
                EmailAddress = user.Email.Address,
                IsVerified = user.Email.IsVerified,
                PasswordHash = user.Password.HashedPassword,
                HashAlgorithm = user.Password.HashAlgorithm.ToString()
            };
        }

        public static User ToDomain(UserModel user)
        {
            if (!Enum.TryParse<HashAlgorithm>(user.HashAlgorithm, out var hashAlgorithm)
                || !Enum.IsDefined(typeof(HashAlgorithm), hashAlgorithm))
                throw new UnknownHashAlgorithmException();

            return new User(new Password(hashAlgorithm, user.PasswordHash),
                new Email(user.EmailAddress, user.IsVerified), user.UserId);
        }
    }
}
