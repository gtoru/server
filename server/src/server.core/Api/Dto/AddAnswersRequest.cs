using System.Collections.Generic;

namespace server.core.Api.Dto
{
    public class AddAnswersRequest
    {
        public List<TaskAnswer> Answers { get; set; }
    }

    public class TaskAnswer
    {
        public int TaskNumber { get; set; }
        public string Answer { get; set; }
    }
}
