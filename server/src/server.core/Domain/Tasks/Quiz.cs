using System;
using System.Collections.Generic;

namespace server.core.Domain.Tasks
{
    public class Quiz
    {
        public Guid QuizId { get; set; }

        public List<VariantTask> Tasks { get; set; }

        public bool Locked { get; set; } = false;
    }
}
