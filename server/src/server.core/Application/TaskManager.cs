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

        public static async Task<VariantTask> GetTaskAsync(
            IUnitOfWork unitOfWork,
            Guid taskId)
        {
            var task = await unitOfWork.Tasks.FindTaskAsync(taskId);

            return task;
        }

        public static async Task<Quiz> GetQuizAsync(
            IUnitOfWork unitOfWork,
            Guid quizId)
        {
            var quiz = await unitOfWork.Quizzes.FindQuizAsync(quizId);

            return quiz;
        }

        public static async Task<Quiz> AddQuizAsync(
            IUnitOfWork unitOfWork,
            IEnumerable<Guid> taskIds)
        {
            var tasks =
                await unitOfWork.Tasks.GetBySpecAsync(t => taskIds.Contains(t.TaskId));

            var quiz = Quiz.CreateNew(tasks.ToList());

            await unitOfWork.Quizzes.AddQuizAsync(quiz);

            return quiz;
        }

        public static async Task<IEnumerable<Quiz>> GetAllQuizzesAsync(IUnitOfWork unitOfWork)
        {
            return await unitOfWork.Quizzes.GetAllAsync();
        }

        public static async Task<IEnumerable<VariantTask>> GetAllTasksAsync(IUnitOfWork unitOfWork)
        {
            return await unitOfWork.Tasks.GetAllAsync();
        }
    }
}
