using System;

namespace server.core.Domain
{
    public class PersonalInfo
    {
        public PersonalInfo()
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

        public string Name { get; set; }
        public DateTime Birthday { get; set; }
        public string Address { get; set; }
        public string Occupation { get; set; }
        public string Employer { get; set; }
    }
}
