using System;
using server.core.Domain;

namespace server.core.Api.Dto
{
    public class AuthenticationResponse
    {
        public string Token { get; set; }

        public PersonalInfo PersonalInfo { get; set; }

        public Guid UserId { get; set; }
    }
}
