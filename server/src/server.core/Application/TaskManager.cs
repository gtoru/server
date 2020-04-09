using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using server.core.Domain.Tasks;
using server.core.Infrastructure;

namespace server.core.Application
{
    public static class TaskManager
    {
        public static async Task<VariantTask> AddTaskAsync(
            IUnitOfWork unitOfWork,
            string question,
            string answer,
            IEnumerable<string> variants)
        {
            var task = VariantTask.CreateNew(
                question,
                answer,
                variants.ToList());

            await unitOfWork.Tasks.AddTaskAsync(task);

            return task;
        }

        public static async Task<Quiz> AddQuizAsync(
            IUnitOfWork unitOfWork,
            IEnumerable<Guid> taskIds)
        {
            var tasks =
                await Task.WhenAll(taskIds.Select(async id => await unitOfWork.Tasks.FindTaskAsync(id)));

            var quiz = Quiz.CreateNew(tasks.ToList());

            await unitOfWork.Quizzes.AddQuizAsync(quiz);

            return quiz;
        }

        public static IQueryable<Quiz> GetAllQuizzes(IUnitOfWork unitOfWork)
        {
            return unitOfWork.Quizzes.GetAllAsQueryable();
        }

        public static IAsyncEnumerable<Quiz> GetAllQuizzesEnumerableAsync(IUnitOfWork unitOfWork)
        {
            return unitOfWork.Quizzes.GetAllEnumerableAsync();
        }

        public static IQueryable<VariantTask> GetAllTasks(IUnitOfWork unitOfWork)
        {
            return unitOfWork.Tasks.GetAllAsQueryable();
        }

        public static IAsyncEnumerable<VariantTask> GetAllTasksEnumerableAsync(IUnitOfWork unitOfWork)
        {
            return unitOfWork.Tasks.GetAllEnumerableAsync();
        }
    }
}
