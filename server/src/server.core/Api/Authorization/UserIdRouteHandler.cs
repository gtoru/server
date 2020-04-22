using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace server.core.Api.Authorization
{
    public class UserIdRouteHandler : AuthorizationHandler<UserIdRouteRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserIdRouteHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            UserIdRouteRequirement requirement)
        {
            var userId = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (userId == null)
                return Task.CompletedTask;

            if (!_httpContextAccessor.HttpContext.Request.Path.StartsWithSegments(
                $"{requirement.UrlPrefix}/{userId}"))
                return Task.CompletedTask;

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
