using Microsoft.AspNetCore.Authorization;

namespace server.core.Api.Authorization
{
    public class UserIdRouteRequirement : IAuthorizationRequirement
    {
        public string UrlPrefix { get; }

        public UserIdRouteRequirement(string urlPrefix)
        {
            UrlPrefix = urlPrefix;
        }
    }
}
