using System;
using server.core.Domain;

namespace server.core.Api.Dto
{
    public class RegistrationRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime Birthday { get; set; }
        public string Address { get; set; }
        public string Occupation { get; set; }
        public string Employer { get; set; }

        public PersonalInfo ExtractPersonalInfo()
        {
            return new PersonalInfo(
                Name,
                Birthday,
                Address,
                Occupation,
                Employer);
        }
    }
}
