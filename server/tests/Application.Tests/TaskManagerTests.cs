using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using server.core.Application;
using server.core.Domain.Storage;
using server.core.Domain.Tasks;
using server.core.Infrastructure;

namespace Application.Tests
{
    [TestFixture]
    public class TaskManagerTests
    {
        [SetUp]
        public void SetUp()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
        }

        private IUnitOfWork _unitOfWork;

        [Test]
        public async Task Should_call_quiz_add()
        {
            var id = Guid.NewGuid();
            var task = VariantTask.CreateNew("foo", "bar", new List<string>(), 2);
            _unitOfWork.Tasks.Returns(Substitute.For<ITaskRepository>());
            _unitOfWork.Quizzes.Returns(Substitute.For<IQuizRepository>());

            _unitOfWork.Tasks
                .GetBySpecAsync(Arg.Any<Expression<Func<VariantTask, bool>>>())
                .Returns(new List<VariantTask> {task});

            var addedQuiz = await TaskManager.AddQuizAsync(_unitOfWork, "TestQuiz", new[] {id});

            await _unitOfWork.Tasks.Received(1).GetBySpecAsync(Arg.Any<Expression<Func<VariantTask, bool>>>());
            await _unitOfWork.Quizzes.Received(1).AddQuizAsync(Arg.Any<Quiz>());
            addedQuiz.Tasks.Single().Task.Should().Be(task);
        }

        [Test]
        public async Task Should_call_task_add()
        {
            var question = "foo";
            var answer = "bar";
            var variants = new List<string>();
            var weight = 2;

            Expression<Predicate<VariantTask>> isEqual = task => task.Answer == answer &&
                                                                 task.Question == question &&
                                                                 task.Variants.SequenceEqual(variants) &&
                                                                 task.Weight == weight;

            _unitOfWork.Tasks.Returns(Substitute.For<ITaskRepository>());
            _unitOfWork.Tasks.AddTaskAsync(Arg.Is(isEqual)).Returns(Task.CompletedTask);

            var addedTask = await TaskManager.AddTaskAsync(_unitOfWork, question, answer, variants, weight);

            await _unitOfWork.Tasks.Received(1).AddTaskAsync(Arg.Is(isEqual));
            isEqual.Compile().Invoke(addedTask).Should().BeTrue();
        }
    }
}
