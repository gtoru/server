namespace server.core.Domain.Authentication
{
    public class Email
    {
        public Email(string address, bool isVerified)
        {
            Address = address;
            IsVerified = isVerified;
        }

        public bool IsVerified { get; private set; }
        public string Address { get; }

        public static Email Create(string address)
        {
            return new Email(address, false);
        }

        public void Verify()
        {
            IsVerified = true;
        }
    }
}
