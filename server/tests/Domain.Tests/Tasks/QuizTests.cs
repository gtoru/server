using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using server.core.Domain.Error;
using server.core.Domain.Tasks;

namespace Domain.Tests.Tasks
{
    [TestFixture]
    public class QuizTests
    {
        [SetUp]
        public void SetUp()
        {
            _task = VariantTask.CreateNew(
                "foo",
                "bar",
                new List<string> {"baz", "bar"});
        }

        private VariantTask _task;

        [Test]
        public void Should_create_quiz()
        {
            var quiz = Quiz.CreateNew(new List<VariantTask> {_task});

            quiz.Tasks.Single().Should().BeEquivalentTo(_task);
            quiz.Locked.Should().BeFalse();
        }

        [Test]
        public void Should_lock_quiz_and_tasks()
        {
            var quiz = Quiz.CreateNew(new List<VariantTask> {_task});

            quiz.Lock();
            quiz.Locked.Should().BeTrue();
            quiz.Tasks.All(t => t.Locked).Should().BeTrue();
        }

        [Test]
        public void Should_not_allow_empty_quiz_creation()
        {
            Action creation = () => Quiz.CreateNew(new List<VariantTask>());

            creation.Should().Throw<EmptyTaskListException>();
        }
    }
}
