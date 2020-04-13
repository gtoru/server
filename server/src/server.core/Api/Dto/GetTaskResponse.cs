using System;
using System.Collections.Generic;

namespace server.core.Api.Dto
{
    public class GetTaskResponse
    {
        public Guid TaskId { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public List<string> Variants { get; set; }
    }
}
