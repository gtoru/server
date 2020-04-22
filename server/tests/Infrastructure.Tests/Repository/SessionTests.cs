using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using server.core.Domain;
using server.core.Domain.Tasks;
using server.core.Infrastructure;
using server.core.Infrastructure.Error.NotFound;

namespace Infrastructure.Tests.Repository
{
    [TestFixture]
    public class SessionTests
    {
        private const string QuizName = "TestQuiz";
        private IUnitOfWork _unitOfWork;
        private User _user;
        private Quiz _quiz;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _unitOfWork = new UnitOfWork(await DbSetUpFixture.GetContextAsync());
            _user = User.CreateNew(
                "email",
                "password",
                new PersonalInfo("John Doe", DateTime.UtcNow, "", "", ""));
            _quiz = Quiz.CreateNew(
                QuizName,
                new List<VariantTask>
                {
                    VariantTask.CreateNew("foo", "bar", new List<string>(), 2),
                    VariantTask.CreateNew("baz", "quux", new List<string>(), 2)
                });

            await _unitOfWork.Quizzes.AddQuizAsync(_quiz);
            await _unitOfWork.Users.AddUserAsync(_user);
            await _unitOfWork.SaveAsync();
        }

        [Test]
        public async Task Should_add_quiz_test_session_on_start()
        {
            var user = await _unitOfWork.Users.FindUserAsync(_user.UserId);
            var quiz = await _unitOfWork.Quizzes.FindQuizAsync(_quiz.QuizId);

            user.StartNewSession(quiz);
            await _unitOfWork.SaveAsync();
            var foundSession = await _unitOfWork.TestSessions.FindTestSessionAsync(user.CurrentSession.SessionId);

            foundSession.Quiz.Should().BeEquivalentTo(quiz);
            foundSession.UserId.Should().Be(user.UserId);
            foundSession.PossibleResult.Should().Be(2);
            foundSession.IsFinished.Should().BeFalse();
        }

        [Test]
        public async Task Should_find_started_session()
        {
            var foundSession = await _unitOfWork.TestSessions.FindTestSessionAsync(_user.CurrentSession.SessionId);

            foundSession.Should().BeEquivalentTo(_user.CurrentSession);
        }

        [Test]
        public void Should_throw_when_session_not_found()
        {
            Func<Task> sessionSearch = async () => await _unitOfWork.TestSessions.FindTestSessionAsync(Guid.NewGuid());

            sessionSearch.Should().Throw<SessionNotFoundException>();
        }
    }
}
