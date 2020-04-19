namespace server.core.Api.Authorization
{
    public static class AuthorizationPolicy
    {
        public const string OnlyAdmins = "OnlyAdminsPolicy";
        public const string EveryoneAllowed = "EveryoneAllowedPolicy";
        public const string CanOnlyAccessOwnSessions = "CanOnlyAccessOwnSessionsPolicy";
    }
}
