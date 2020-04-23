using System;

namespace server.core.Api.Dto
{
    public class GetResultResponse
    {
        public Guid TestSessionId { get; set; }
        public int Result { get; set; }
    }
}
