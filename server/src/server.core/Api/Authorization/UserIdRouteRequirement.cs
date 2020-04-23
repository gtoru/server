using Microsoft.AspNetCore.Authorization;

namespace server.core.Api.Authorization
{
    public class UserIdRouteRequirement : IAuthorizationRequirement
    {
        public UserIdRouteRequirement(string urlPrefix)
        {
            UrlPrefix = urlPrefix;
        }

        public string UrlPrefix { get; }
    }
}
