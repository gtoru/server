using System;
using server.core.Domain;

namespace server.core.Api.Dto
{
    public class SessionInfo
    {
        public Guid UserId { get; set; }

        public string Email { get; set; }

        public PersonalInfo PersonalInfo { get; set; }
    }
}
