using System;
using System.Collections.Generic;

namespace server.core.Api.Dto
{
    public class CreateQuizRequest
    {
        public string QuizName { get; set; }
        public List<Guid> Tasks { get; set; }
    }
}
