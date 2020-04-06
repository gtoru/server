using Microsoft.AspNetCore.Authorization;
using server.core.Domain.Authentication;

namespace server.core.Api.Authorization
{
    public class AccessLevelRequirement : IAuthorizationRequirement
    {
        public AccessLevelRequirement(AccessLevel accessLevel)
        {
            AccessLevel = accessLevel;
        }

        public AccessLevel AccessLevel { get; }
    }
}
