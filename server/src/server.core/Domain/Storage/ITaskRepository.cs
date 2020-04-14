using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using server.core.Domain.Tasks;

namespace server.core.Domain.Storage
{
    public interface ITaskRepository
    {
        Task AddTaskAsync(VariantTask task);
        Task<VariantTask> FindTaskAsync(Guid taskId);
        Task<List<VariantTask>> GetAllAsync();
        Task<List<VariantTask>> GetBySpecAsync(Expression<Func<VariantTask, bool>> spec);
    }
}
