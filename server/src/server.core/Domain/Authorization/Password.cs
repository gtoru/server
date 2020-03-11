using System;
using System.ComponentModel;

namespace server.core.Domain.Authorization
{
    public readonly struct Password
    {
        public readonly HashAlgorithm HashAlgorithm;
        public readonly string HashedPassword;

        private Password(HashAlgorithm hashAlgorithm, string hashedPassword)
        {
            HashAlgorithm = hashAlgorithm;
            HashedPassword = hashedPassword;
        }

        public static Password Create(HashAlgorithm hashAlgorithm, string password)
        {
            if (password.Length > 50)
                throw new ArgumentException("password should not be more than 50 symbols");

            switch (hashAlgorithm)
            {
                case HashAlgorithm.BCrypt:
                    return new Password(
                        HashAlgorithm.BCrypt,
                        BCrypt.Net.BCrypt.EnhancedHashPassword(password));
                default:
                    throw new InvalidEnumArgumentException($"unknown hash algorithm: {hashAlgorithm}");
            }
        }

        public bool Verify(string password)
        {
            switch (HashAlgorithm)
            {
                case HashAlgorithm.BCrypt:
                    return BCrypt.Net.BCrypt.EnhancedVerify(password, HashedPassword);
                default:
                    throw new InvalidEnumArgumentException($"unknown hashing algorithm: {HashAlgorithm}");
            }
        }
    }
}
