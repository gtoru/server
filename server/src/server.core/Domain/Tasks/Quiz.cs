using System;
using System.Collections.Generic;
using System.Linq;
using server.core.Domain.Error;
using server.core.Domain.Tasks.Helpers;

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
            Tasks = tasks.Select(t => new QuizTask
            {
                Quiz = this,
                QuizId = quizId,
                Task = t,
                TaskId = t.TaskId
            }).ToList();
        }

        public Guid QuizId { get; set; }

        public List<QuizTask> Tasks { get; set; }

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

            foreach (var task in Tasks) task.Task.Lock();
        }
    }
}
