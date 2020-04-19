using System;
using System.Collections.Generic;

namespace server.core.Api.Dto
{
    public class TestSessionResponse
    {
        public Guid TestSessionId { get; set; }

        public List<TaskResponse> Tasks { get; set; }
    }

    public class TaskResponse
    {
        public string Question { get; set; }

        public List<string> Answers { get; set; }
    }
}
