using System.Collections.Generic;

namespace server.core.Api.Dto
{
    public class UserStatsResponse
    {
        public List<UserStats> UserStats { get; set; }
    }

    public class UserStats
    {
        public string Email { get; set; }
        public int Result { get; set; }
    }
}
