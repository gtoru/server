using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using server.core.Domain.Authentication;

namespace server.core.Api.Authorization
{
    public class AccessLevelHandler : AuthorizationHandler<AccessLevelRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            AccessLevelRequirement requirement)
        {
            var accessLevel = context.User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Role);

            if (accessLevel == null)
                return Task.CompletedTask;

            if (!Enum.TryParse<AccessLevel>(accessLevel.Value, out var level))
                return Task.CompletedTask;

            if (level <= requirement.AccessLevel)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
