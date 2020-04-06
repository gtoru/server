using Newtonsoft.Json;

namespace server.core.Api.Dto
{
    public class AuthenticationRequest
    {
        [JsonRequired]
        public string Email { get; set; }

        [JsonRequired]
        public string Password { get; set; }
    }
}
