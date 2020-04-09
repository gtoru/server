using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using server.core.Domain.Tasks;

namespace server.core.Domain.Storage
{
    public interface ITaskRepository
    {
        Task AddTaskAsync(VariantTask task);
        Task<VariantTask> FindTaskAsync(Guid taskId);
        IQueryable<VariantTask> GetAllAsQueryable();
        Task<List<VariantTask>> GetAllAsync();
    }
}
