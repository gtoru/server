using System;
using System.Threading.Tasks;
using server.core.Domain.Tasks;

namespace server.core.Domain.Storage
{
    public interface IQuizRepository
    {
        Task AddQuizAsync(Quiz quiz);
        Task<Quiz> FindQuizAsync(Guid quizId);
    }
}
