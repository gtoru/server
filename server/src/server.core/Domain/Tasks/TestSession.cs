using System;

namespace server.core.Domain.Tasks
{
    public class TestSession
    {
        public Guid SessionId { get; set; }
        public DateTime Started { get; set; }
        public bool IsFinished { get; set; }
        public int Result { get; set; }
        public int PossibleResult { get; set; }
        public Guid QuizId { get; set; }
        public string[] Answers { get; set; }
    }
}
