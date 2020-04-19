using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using server.core.Domain.Tasks;
using server.core.Infrastructure;
using server.core.Infrastructure.Error.NotFound;

namespace Infrastructure.Tests.Repository
{
    [TestFixture]
    public class QuizTests
    {
        [SetUp]
        public async Task SetUp()
        {
            _unitOfWork = await DbSetUpFixture.GetUnitOfWorkAsync();
        }

        [TearDown]
        public async Task TearDown()
        {
            await _unitOfWork.SaveAsync();
        }

        private VariantTask _firstTask;
        private VariantTask _secondTask;
        private Quiz _quiz;
        private const string QuizName = "TestQuiz";
        private const string FirstAnswer = "bar";
        private const string SecondAnswer = "baz";
        private IUnitOfWork _unitOfWork;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _firstTask = VariantTask.CreateNew(
                "foo",
                FirstAnswer,
                new List<string> {"baz", "quuz", FirstAnswer});

            _secondTask = VariantTask.CreateNew(
                "bar",
                SecondAnswer,
                new List<string> {SecondAnswer, "quux", "baq"});

            _quiz = Quiz.CreateNew(QuizName, new List<VariantTask> {_firstTask, _secondTask});

            var unitOfWork = await DbSetUpFixture.GetUnitOfWorkAsync();

            await unitOfWork.Tasks.AddTaskAsync(_firstTask);
            await unitOfWork.Tasks.AddTaskAsync(_secondTask);
            await unitOfWork.Quizzes.AddQuizAsync(_quiz);
            await unitOfWork.SaveAsync();
        }

        [Test]
        public async Task Should_be_able_to_create_quiz_with_same_tasks()
        {
            var firstTask = await _unitOfWork.Tasks.FindTaskAsync(_firstTask.TaskId);
            var secondQuiz = Quiz.CreateNew(QuizName, new List<VariantTask> {firstTask});

            await _unitOfWork.Quizzes.AddQuizAsync(secondQuiz);
            await _unitOfWork.SaveAsync();

            var firstQuiz = await _unitOfWork.Quizzes.FindQuizAsync(_quiz.QuizId);
            secondQuiz = await _unitOfWork.Quizzes.FindQuizAsync(secondQuiz.QuizId);

            firstQuiz.Tasks[0].Task.Should().BeEquivalentTo(secondQuiz.Tasks[0].Task);
        }

        [Test]
        public void Should_create_tasks_on_quiz_creation()
        {
            Func<Task> taskSearch = async () =>
            {
                await _unitOfWork.Tasks.FindTaskAsync(_quiz.Tasks[0].TaskId);
                await _unitOfWork.Tasks.FindTaskAsync(_quiz.Tasks[1].TaskId);
            };

            taskSearch.Should().NotThrow<TaskNotFoundException>();
        }

        [Test]
        public async Task Should_lock_quiz_and_tasks()
        {
            var quiz = await _unitOfWork.Quizzes.FindQuizAsync(_quiz.QuizId);

            quiz.Lock();
            await _unitOfWork.SaveAsync();

            quiz = await _unitOfWork.Quizzes.FindQuizAsync(_quiz.QuizId);
            quiz.Locked.Should().BeTrue();

            var firstTask = await _unitOfWork.Tasks.FindTaskAsync(quiz.Tasks[0].TaskId);
            firstTask.Locked.Should().BeTrue();

            var secondTask = await _unitOfWork.Tasks.FindTaskAsync(quiz.Tasks[1].TaskId);
            secondTask.Locked.Should().BeTrue();
        }

        [Test]
        public async Task Should_return_all_quizzes()
        {
            var quiz = await _unitOfWork.Quizzes.GetAllAsync();

            quiz.Count.Should().Be(2);
        }

        [Test]
        public void Should_throw_when_quiz_not_found()
        {
            Func<Task> quizSearch = async () => await _unitOfWork.Quizzes.FindQuizAsync(Guid.NewGuid());

            quizSearch.Should().Throw<QuizNotFoundException>();
        }
    }
}
