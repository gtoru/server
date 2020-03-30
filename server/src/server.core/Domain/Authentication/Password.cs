using server.core.Domain.Error;

namespace server.core.Domain.Authentication
{
    public class Password
    {
        public Password(HashAlgorithm hashAlgorithm, string hashedPassword)
        {
            HashAlgorithm = hashAlgorithm;
            HashedPassword = hashedPassword;
        }

        public HashAlgorithm HashAlgorithm { get; }
        public string HashedPassword { get; }

        public static Password Create(HashAlgorithm hashAlgorithm, string password)
        {
            if (password.Length > 50)
                throw new PasswordTooLongException();

            switch (hashAlgorithm)
            {
                case HashAlgorithm.BCrypt:
                    return new Password(
                        HashAlgorithm.BCrypt,
                        BCrypt.Net.BCrypt.EnhancedHashPassword(password));
                default:
                    throw new UnknownHashAlgorithmException();
            }
        }

        public bool Verify(string password)
        {
            switch (HashAlgorithm)
            {
                case HashAlgorithm.BCrypt:
                    return BCrypt.Net.BCrypt.EnhancedVerify(password, HashedPassword);
                default:
                    throw new UnknownHashAlgorithmException();
            }
        }
    }
}
