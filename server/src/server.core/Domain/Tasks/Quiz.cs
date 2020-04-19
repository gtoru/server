using System;
using System.Collections.Generic;
using server.core.Domain.Error;
using server.core.Domain.Tasks.Helpers;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace server.core.Domain.Tasks
{
    public class Quiz
    {
        private Quiz()
        {
        }

        private Quiz(List<QuizTask> tasks)
        {
            Tasks = tasks;
        }

        public string QuizName { get; private set; }

        public Guid QuizId { get; private set; }

        public List<QuizTask> Tasks { get; private set; } = new List<QuizTask>();

        public bool Locked { get; private set; }

        public static Quiz CreateNew(string quizName, List<VariantTask> tasks)
        {
            if (tasks.Count == 0)
                throw new EmptyTaskListException();

            var quiz = new Quiz {QuizName = quizName};


            foreach (var task in tasks)
            {
                var quizTask = new QuizTask
                {
                    Quiz = quiz,
                    Task = task
                };

                quiz.Tasks.Add(quizTask);
                task.Quizzes.Add(quizTask);
            }

            return quiz;
        }

        public void Lock()
        {
            Locked = true;

            foreach (var task in Tasks) task.Task.Lock();
        }
    }
}
