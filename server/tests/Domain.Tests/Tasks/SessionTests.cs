using System;
using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using server.core.Domain;
using server.core.Domain.Error;
using server.core.Domain.Misc;
using server.core.Domain.Tasks;

namespace Domain.Tests.Tasks
{
    [TestFixture]
    public class SessionTests
    {
        private const string QuizName = "TestQuiz";
        [SetUp]
        public void SetUp()
        {
            _tasks = new List<VariantTask>
            {
                VariantTask.CreateNew("foo", "bar", new List<string> {"baz", "bar", "quux"}),
                VariantTask.CreateNew("baz", "foo", new List<string> {"foo", "bar"})
            };

            _user = User.CreateNew(
                "foo@bar.baz",
                "qwerty",
                new PersonalInfo("John Doe", DateTime.UtcNow, "", "", ""));
            _quiz = Quiz.CreateNew(QuizName, _tasks);
            _timeProvider = Substitute.For<ITimeProvider>();

            _startTime = new DateTime(1970, 01, 01);
            _timeProvider.GetCurrent().Returns(_startTime);
        }

        private User _user;
        private Quiz _quiz;
        private List<VariantTask> _tasks;
        private ITimeProvider _timeProvider;
        private DateTime _startTime;

        [Test]
        public void Should_fail_to_add_answer_to_non_existing_question()
        {
            var session = TestSession.CreateNew(_user, _quiz, _timeProvider);

            Action answer = () => session.Answer(10, "PANIC");

            answer.Should().Throw<TaskNumberOutOfRangeException>();
        }

        [Test]
        public void Should_fail_to_answer_if_time_is_over()
        {
            var session = TestSession.CreateNew(_user, _quiz, _timeProvider);

            var checkTime = new DateTime(1971, 01, 01);
            _timeProvider.GetCurrent().Returns(checkTime);

            Action answer = () => session.Answer(0, "foo");

            answer.Should().Throw<TestSessionAlreadyFinishedException>();
        }

        [Test]
        public void Should_set_session_to_finished_if_answering_after_time_expired()
        {
            var session = TestSession.CreateNew(_user, _quiz, _timeProvider);

            var checkTime = new DateTime(1971, 01, 01);
            _timeProvider.GetCurrent().Returns(checkTime);

            session.IsFinished.Should().BeFalse();

            try
            {
                session.Answer(0, "foo");
            }
            catch (TestSessionAlreadyFinishedException)
            {
            }

            session.IsFinished.Should().BeTrue();
        }

        [Test]
        public void Should_set_test_result_on_finish()
        {
            var session = TestSession.CreateNew(_user, _quiz, _timeProvider);

            session.Answer(0, "bar");

            session.Finish();

            session.Result.Should().Be(1);
        }

        [Test]
        public void Should_set_to_finished_on_finish()
        {
            var session = TestSession.CreateNew(_user, _quiz, _timeProvider);

            session.Finish();

            session.IsFinished.Should().BeTrue();
        }

        [Test]
        public void Should_start_new_session()
        {
            var session = TestSession.CreateNew(_user, _quiz, _timeProvider);

            session.Started.Should().Be(_startTime);
            session.Answers.Count.Should().Be(_quiz.Tasks.Count);
            session.IsFinished.Should().BeFalse();
            session.Quiz.Should().Be(_quiz);
        }
    }
}
