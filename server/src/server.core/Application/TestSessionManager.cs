using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using server.core.Domain.Error;
using server.core.Domain.Tasks;
using server.core.Infrastructure;

namespace server.core.Application
{
    public static class TestSessionManager
    {
        public static async Task<TestSession> StartTestSessionAsync(IUnitOfWork unitOfWork, Guid userId, Guid quizId)
        {
            var quiz = await unitOfWork.Quizzes.FindQuizAsync(quizId);
            var user = await unitOfWork.Users.FindUserAsync(userId);

            user.StartNewSession(quiz);

            return user.CurrentSession;
        }

        public static async Task<TestSession> GetActiveSession(IUnitOfWork unitOfWork, Guid userId)
        {
            var user = await unitOfWork.Users.FindUserAsync(userId);

            var currentSession = user.GetActiveSession();

            return currentSession;
        }

        public static async Task AddAnswersAsync(IUnitOfWork unitOfWork, Guid userId, IEnumerable<(int, string)> answers)
        {
            var user = await unitOfWork.Users.FindUserAsync(userId);

            foreach (var (taskNumber, taskGuess) in answers)
                user.CurrentSession.Answer(taskNumber, taskGuess);
        }

        public static async Task<IEnumerable<(Guid sessionId, int result)>> GetResultsAsync(IUnitOfWork unitOfWork, Guid userId)
        {
            var user = await unitOfWork.Users.FindUserAsync(userId);

            var result = new List<(Guid sessionId, int result)>();

            foreach (var session in user.TestSessions)
            {
                if (session.IsFinished || session.Expired())
                    result.Add((session.SessionId, session.GetResult()));
            }

            return result;
        }

        public static async Task<(Guid sessionId, int result)> EndCurrentSessionAsync(IUnitOfWork unitOfWork, Guid userId)
        {
            var user = await unitOfWork.Users.FindUserAsync(userId);

            var currentSession = user.GetActiveSession();

            currentSession.Finish();
            return (currentSession.SessionId, currentSession.GetResult());
        }
    }
}
