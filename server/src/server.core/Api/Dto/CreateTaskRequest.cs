using System.Collections.Generic;
using Newtonsoft.Json;

namespace server.core.Api.Dto
{
    public class CreateTaskRequest
    {
        [JsonRequired]
        public string Question { get; set; }

        [JsonRequired]
        public string Answer { get; set; }

        [JsonRequired]
        public int Weight { get; set; }

        public List<string> Variants { get; set; }
    }
}
