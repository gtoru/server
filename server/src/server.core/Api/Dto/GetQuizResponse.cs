using System;
using System.Collections.Generic;

namespace server.core.Api.Dto
{
    public class GetQuizResponse
    {
        public string QuizName { get; set; }

        public Guid QuizId { get; set; }

        public List<GetTaskResponse> Tasks { get; set; }
    }
}
