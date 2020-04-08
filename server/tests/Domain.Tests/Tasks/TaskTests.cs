using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using server.core.Domain.Tasks;

namespace Domain.Tests.Tasks
{
    [TestFixture]
    public class TaskTests
    {
        [SetUp]
        public void SetUp()
        {
            _variants = new List<string> {VariantA, VariantB, Answer};
        }

        private const string Question = "How much is the fish?";
        private const string Answer = "foo";
        private const string VariantA = "bar";
        private const string VariantB = "baz";

        private List<string> _variants;

        [Test]
        public void Should_create_task()
        {
            var task = VariantTask.CreateNew(
                Question,
                Answer,
                _variants);

            task.Question.Should().BeEquivalentTo(Question);
            task.Answer.Should().BeEquivalentTo(Answer);
            task.Variants.Should().BeEquivalentTo(_variants);
            task.Locked.Should().BeFalse();
        }

        [Test]
        public void Should_lock_task_after_lock_call()
        {
            var task = VariantTask.CreateNew(
                Question,
                Answer,
                _variants);

            task.Lock();

            task.Locked.Should().BeTrue();
        }
    }
}
