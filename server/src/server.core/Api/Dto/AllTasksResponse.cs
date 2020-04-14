using System.Collections.Generic;

namespace server.core.Api.Dto
{
    public class AllTasksResponse
    {
        public List<GetTaskResponse> Tasks { get; set; }
    }
}
