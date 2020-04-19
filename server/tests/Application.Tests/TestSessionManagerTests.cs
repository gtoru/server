using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using server.core.Application;
using server.core.Domain;
using server.core.Domain.Storage;
using server.core.Domain.Tasks;
using server.core.Infrastructure;

namespace Application.Tests
{
    [TestFixture]
    public class TestSessionManagerTests
    {
        [SetUp]
        public void SetUp()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _unitOfWork.Quizzes.Returns(Substitute.For<IQuizRepository>());
            _unitOfWork.Tasks.Returns(Substitute.For<ITaskRepository>());
            _unitOfWork.Users.Returns(Substitute.For<IUserRepository>());
            _unitOfWork.TestSessions.Returns(Substitute.For<ITestSessionRepository>());

            _quiz = Quiz.CreateNew(
                "",
                new List<VariantTask>
                {
                    VariantTask.CreateNew("", "abc", new List<string>()),
                    VariantTask.CreateNew("", "def", new List<string>())
                });

            _user = User.CreateNew(
                "",
                "",
                new PersonalInfo("", DateTime.Now, "", "", ""));
        }

        private IUnitOfWork _unitOfWork;
        private User _user;
        private Quiz _quiz;

        private async Task StartNewSessionAsync()
        {
            _unitOfWork.Quizzes.FindQuizAsync(Arg.Is(_quiz.QuizId)).Returns(_quiz);
            _unitOfWork.Users.FindUserAsync(Arg.Is(_user.UserId)).Returns(_user);

            await TestSessionManager.StartTestSessionAsync(_unitOfWork, _user.UserId, _quiz.QuizId);
        }

        [Test]
        public async Task Should_add_answers_and_return_result()
        {
            await StartNewSessionAsync();

            await TestSessionManager.AddAnswersAsync(_unitOfWork, _user.UserId, new List<(int, string)> {(0, "abc")});

            var result = (await TestSessionManager.GetResultsAsync(_unitOfWork, _user.UserId)).ToList();

            result.Single().result.Should().Be(1);
            result.Single().sessionId.Should().Be(_user.CurrentSession.SessionId);
        }

        [Test]
        public async Task Should_return_active_session()
        {
            await StartNewSessionAsync();

            Func<Task> sessionGet = async () => await TestSessionManager.GetActiveSession(_unitOfWork, _user.UserId);

            sessionGet.Should().NotThrow();
        }

        [Test]
        public async Task Should_start_new_session()
        {
            await StartNewSessionAsync();

            _user.HasActiveSession().Should().BeTrue();
        }
    }
}
