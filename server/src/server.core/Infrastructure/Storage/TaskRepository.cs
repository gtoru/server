using System;
using System.Threading.Tasks;
using server.core.Domain.Storage;
using server.core.Domain.Tasks;
using server.core.Infrastructure.Error.AlreadyExists;
using server.core.Infrastructure.Error.NotFound;

namespace server.core.Infrastructure.Storage
{
    public class TaskRepository : ITaskRepository
    {
        private readonly AppDbContext _dbContext;

        public TaskRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddTaskAsync(VariantTask task)
        {
            var foundTask = await _dbContext.Tasks.FindAsync(task.TaskId);
            if (foundTask != null)
                throw new TaskAlreadyExistsException();

            await _dbContext.Tasks.AddAsync(task);
        }

        public async Task<VariantTask> FindTaskAsync(Guid taskId)
        {
            var foundTask = await _dbContext.Tasks.FindAsync(taskId);

            if (foundTask == null)
                throw new TaskNotFoundException();

            return foundTask;
        }
    }
}
