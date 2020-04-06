using System;
using Newtonsoft.Json;
using server.core.Domain;

namespace server.core.Api.Dto
{
    public class RegistrationRequest
    {
        [JsonRequired]
        public string Name { get; set; }

        [JsonRequired]
        public string Email { get; set; }

        [JsonRequired]
        public string Password { get; set; }

        [JsonRequired]
        public DateTime Birthday { get; set; }

        [JsonRequired]
        public string Address { get; set; }

        [JsonRequired]
        public string Occupation { get; set; }

        [JsonRequired]
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
