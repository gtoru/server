using System;
using System.Collections.Generic;
using server.core.Domain.Tasks.Helpers;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace server.core.Domain.Tasks
{
    public class VariantTask
    {
        private VariantTask()
        {
        }

        private VariantTask(
            string question,
            string answer,
            List<string> variants)
        {
            Question = question;
            Answer = answer;
            Variants = variants;
        }

        public Guid TaskId { get; private set; }

        public string Question { get; private set; }

        public string Answer { get; private set; }

        public List<string> Variants { get; private set; }

        public bool Locked { get; private set; }

        public List<QuizTask> Quizzes { get; private set; } = new List<QuizTask>();

        public static VariantTask CreateNew(string question, string answer, List<string> variants)
        {
            return new VariantTask(
                question,
                answer,
                variants);
        }

        public bool CheckAnswer(string answer)
        {
            return Answer == answer;
        }

        public void Lock()
        {
            Locked = true;
        }
    }
}
