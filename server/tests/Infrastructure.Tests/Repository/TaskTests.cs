using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using server.core.Domain.Tasks;
using server.core.Infrastructure;
using server.core.Infrastructure.Error.AlreadyExists;
using server.core.Infrastructure.Error.NotFound;

namespace Infrastructure.Tests.Repository
{
    [TestFixture]
    public class TaskTests
    {
        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            var context = await DbSetUpFixture.GetContextAsync();
            _unitOfWork = new UnitOfWork(context);

            _task = VariantTask.CreateNew(
                Question,
                Answer,
                new List<string> {A, B, C});

            await _unitOfWork.Tasks.AddTaskAsync(_task);
            await _unitOfWork.SaveAsync();
        }

        private const string Question = "Foo";
        private const string A = "Bar";
        private const string B = "Baz";
        private const string C = "Qux";
        private const string Answer = B;
        private IUnitOfWork _unitOfWork;
        private VariantTask _task;

        [Test]
        public async Task Should_find_created_task()
        {
            var foundTask = await _unitOfWork.Tasks.FindTaskAsync(_task.TaskId);

            foundTask.Should().BeEquivalentTo(_task);
        }

        [Test]
        public async Task Should_return_true_on_correct_answer()
        {
            var task = await _unitOfWork.Tasks.FindTaskAsync(_task.TaskId);

            task.CheckAnswer(Answer).Should().BeTrue();
        }

        [Test]
        public void Should_throw_when_id_already_exists()
        {
            var duplicate = new VariantTask(
                _task.TaskId,
                Question,
                Answer,
                new List<string> {A, B, C});

            Func<Task> taskAddition = async () => await _unitOfWork.Tasks.AddTaskAsync(duplicate);

            taskAddition.Should().Throw<TaskAlreadyExistsException>();
        }

        [Test]
        public void Should_throw_when_task_not_found()
        {
            Func<Task> taskSearch = async () => await _unitOfWork.Tasks.FindTaskAsync(Guid.NewGuid());

            taskSearch.Should().Throw<TaskNotFoundException>();
        }
    }
}
