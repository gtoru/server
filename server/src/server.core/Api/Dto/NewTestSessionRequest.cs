using System;
using Newtonsoft.Json;

namespace server.core.Api.Dto
{
    public class NewTestSessionRequest
    {
        [JsonRequired]
        public Guid QuizId { get; set; }
    }
}
