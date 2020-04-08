using System;
using System.Collections.Generic;
using server.core.Domain.Error;

namespace server.core.Domain.Tasks
{
    public class Quiz
    {
        public Quiz()
        {
        }

        public Quiz(Guid quizId, List<VariantTask> tasks)
        {
            QuizId = quizId;
            Tasks = tasks;
        }

        public Guid QuizId { get; set; }

        public List<VariantTask> Tasks { get; set; }

        public bool Locked { get; set; }

        public static Quiz CreateNew(List<VariantTask> tasks)
        {
            if (tasks.Count == 0)
                throw new EmptyTaskListException();

            return new Quiz(
                Guid.NewGuid(),
                tasks);
        }

        public void Lock()
        {
            Locked = true;

            foreach (var task in Tasks) task.Lock();
        }
    }
}
