using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using server.core.Domain.Storage;
using server.core.Domain.Tasks;
using server.core.Infrastructure.Error.NotFound;

namespace server.core.Infrastructure.Storage
{
    public class QuizRepository : IQuizRepository
    {
        private readonly AppDbContext _dbContext;

        public QuizRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddQuizAsync(Quiz quiz)
        {
            await _dbContext.Quizzes.AddAsync(quiz);
        }

        public async Task<Quiz> FindQuizAsync(Guid quizId)
        {
            var foundQuiz = await _dbContext.Quizzes
                .Include(m => m.Tasks)
                .ThenInclude(m => m.Task)
                .FirstOrDefaultAsync(m => m.QuizId == quizId);

            if (foundQuiz == null)
                throw new QuizNotFoundException();

            return foundQuiz;
        }
    }
}
