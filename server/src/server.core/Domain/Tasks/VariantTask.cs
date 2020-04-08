using System;
using System.Collections.Generic;

namespace server.core.Domain.Tasks
{
    public class VariantTask
    {
        public VariantTask()
        {
        }

        public VariantTask(
            Guid taskId,
            string question,
            string answer,
            List<string> variants)
        {
            TaskId = taskId;
            Question = question;
            Answer = answer;
            Variants = variants;
        }

        public Guid TaskId { get; set; }

        public string Question { get; set; }

        public string Answer { get; set; }

        public List<string> Variants { get; set; }

        public bool Locked { get; set; }

        public static VariantTask CreateNew(string question, string answer, List<string> variants)
        {
            var id = Guid.NewGuid();
            return new VariantTask(
                id,
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
