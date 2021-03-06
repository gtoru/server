using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using server.core.Domain.Storage;
using server.core.Domain.Tasks;
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
            await _dbContext.Tasks.AddAsync(task);
        }

        public async Task<VariantTask> FindTaskAsync(Guid taskId)
        {
            var foundTask = await _dbContext.Tasks
                .FirstOrDefaultAsync(m => m.TaskId == taskId);

            if (foundTask == null)
                throw new TaskNotFoundException();

            await _dbContext.Entry(foundTask)
                .Collection(m => m.Quizzes)
                .LoadAsync();

            await _dbContext.Entry(foundTask)
                .Collection(m => m.Quizzes)
                .Query()
                .Select(q => q.Quiz)
                .LoadAsync();

            return foundTask;
        }

        public async Task<List<VariantTask>> GetAllAsync()
        {
            return await _dbContext.Tasks.ToListAsync();
        }

        public async Task<List<VariantTask>> GetBySpecAsync(Expression<Func<VariantTask, bool>> spec)
        {
            return await _dbContext.Tasks.Where(spec).ToListAsync();
        }

        public IAsyncEnumerable<VariantTask> GetAllEnumerableAsync()
        {
            return _dbContext.Tasks.AsAsyncEnumerable();
        }
    }
}
