using System;
using System.Collections.Generic;
using System.Linq;
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
                .FirstOrDefaultAsync(m => m.QuizId == quizId);

            if (foundQuiz == null)
                throw new QuizNotFoundException();

            await _dbContext.Entry(foundQuiz)
                .Collection(m => m.Tasks)
                .LoadAsync();

            await _dbContext.Entry(foundQuiz)
                .Collection(m => m.Tasks)
                .Query()
                .Select(m => m.Task)
                .LoadAsync();

            return foundQuiz;
        }

        public async Task<List<Quiz>> GetAllAsync()
        {
            return await _dbContext.Quizzes.ToListAsync();
        }
    }
}
