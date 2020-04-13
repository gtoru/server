using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using server.core.Domain.Tasks;

namespace server.core.Domain.Storage
{
    public interface IQuizRepository
    {
        Task AddQuizAsync(Quiz quiz);
        Task<Quiz> FindQuizAsync(Guid quizId);
        Task<List<Quiz>> GetAllAsync();
    }
}
