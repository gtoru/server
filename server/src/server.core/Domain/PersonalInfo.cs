using System;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace server.core.Domain
{
    public class PersonalInfo
    {
        private PersonalInfo()
        {
        }

        public PersonalInfo(
            string name,
            DateTime birthday,
            string address,
            string occupation,
            string employer)
        {
            Name = name;
            Birthday = birthday;
            Address = address;
            Occupation = occupation;
            Employer = employer;
        }

        public string Name { get; private set; }
        public DateTime Birthday { get; private set; }
        public string Address { get; private set; }
        public string Occupation { get; private set; }
        public string Employer { get; private set; }
    }
}
