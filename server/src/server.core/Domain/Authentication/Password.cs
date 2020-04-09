using server.core.Domain.Error;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace server.core.Domain.Authentication
{
    public class Password
    {
        public Password()
        {
        }

        private Password(HashAlgorithm hashAlgorithm, string hashedPassword)
        {
            HashAlgorithm = hashAlgorithm;
            HashedPassword = hashedPassword;
        }

        public HashAlgorithm HashAlgorithm { get; private set; }
        public string HashedPassword { get; private set; }

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

        public void Verify(string password)
        {
            var passwordVerified = HashAlgorithm switch
            {
                HashAlgorithm.BCrypt =>
                BCrypt.Net.BCrypt.EnhancedVerify(password, HashedPassword),
                _ => throw new UnknownHashAlgorithmException()
            };

            if (!passwordVerified)
                throw new IncorrectPasswordException();
        }
    }
}
