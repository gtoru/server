using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using server.core.Domain;
using server.core.Domain.Authentication;
using server.core.Domain.Error;
using server.core.Domain.Tasks;

namespace Domain.Tests
{
    [TestFixture]
    public class UserTests
    {
        [SetUp]
        public void SetUp()
        {
            _personalInfo = new PersonalInfo(
                "John Doe",
                new DateTime(1970, 01, 01),
                "Over the Rainbow",
                "Believer",
                "Monsters inc.");
        }

        private const string QuizName = "TestQuiz";
        private const string Address = "foo@bar.baz";
        private const string Password = "qwerty";
        private PersonalInfo _personalInfo;

        [Test]
        public void Should_add_answers()
        {
            var user = User.CreateNew(Address, Password, _personalInfo);
            var quiz = Quiz.CreateNew(
                QuizName,
                new List<VariantTask>
                {
                    VariantTask.CreateNew("foo", "bar", new List<string>()),
                    VariantTask.CreateNew("baz", "quux", new List<string>())
                });

            user.StartNewSession(quiz);

            user.CurrentSession.Answer(0, "bar");
            user.CurrentSession.Answer(1, "baz");

            user.CurrentSession.Answers.Count.Should().Be(2);
        }

        [Test]
        public void Should_calculate_score()
        {
            var user = User.CreateNew(Address, Password, _personalInfo);
            var quiz = Quiz.CreateNew(
                QuizName,
                new List<VariantTask>
                {
                    VariantTask.CreateNew("foo", "bar", new List<string>()),
                    VariantTask.CreateNew("baz", "quux", new List<string>())
                });

            user.StartNewSession(quiz);

            user.CurrentSession.Answer(0, "bar");
            user.CurrentSession.Answer(1, "baz");

            user.CurrentSession.Finish();
            user.CurrentSession.Result.Should().Be(1);
        }

        [Test]
        public void Should_add_new_session()
        {
            var user = User.CreateNew(Address, Password, _personalInfo);
            var quiz = Quiz.CreateNew(
                QuizName,
                new List<VariantTask>
                {
                    VariantTask.CreateNew("foo", "bar", new List<string>()),
                    VariantTask.CreateNew("baz", "quux", new List<string>())
                });

            user.StartNewSession(quiz);

            user.HasActiveSession().Should().BeTrue();
            user.TestSessions.Count.Should().Be(1);
            user.TestSessions.Single().Should().Be(user.CurrentSession);
            user.CurrentSession.Quiz.Should().BeEquivalentTo(quiz);
        }

        [Test]
        public void Should_throw_when_starting_new_session_if_has_active_session()
        {
            var user = User.CreateNew(Address, Password, _personalInfo);
            var quiz = Quiz.CreateNew(QuizName, new List<VariantTask> {VariantTask.CreateNew("foo", "bar", new List<string>())});
            user.StartNewSession(quiz);

            Action sessionStart = () => user.StartNewSession(quiz);

            sessionStart.Should().Throw<AlreadyHasActiveSessionException>();
        }

        [Test]
        public void Should_create_with_empty_test_session()
        {
            var user = User.CreateNew(Address, Password, _personalInfo);

            user.TestSessions.Count.Should().Be(0);
        }

        [Test]
        public void Should_create_with_unverified_email()
        {
            var user = User.CreateNew(Address, Password, _personalInfo);

            user.Email.IsVerified.Should().BeFalse();
        }

        [Test]
        public void Should_set_admin_access_rights_for_admins()
        {
            var admin = User.CreateAdmin(Address, Password);

            admin.AccessLevel.Should().Be(AccessLevel.Administrator);
        }

        [Test]
        public void Should_set_correct_email()
        {
            var user = User.CreateNew(Address, Password, _personalInfo);

            user.Email.Address.Should().BeEquivalentTo(Address);
        }

        [Test]
        public void Should_set_correct_password()
        {
            var user = User.CreateNew(Address, Password, _personalInfo);

            Action passwordVerification = () => user.Password.Verify(Password);

            passwordVerification.Should().NotThrow();
        }

        [Test]
        public void Should_set_minimal_access_rights_for_normal_user()
        {
            var user = User.CreateNew(Address, Password, _personalInfo);

            user.AccessLevel.Should().Be(AccessLevel.User);
        }

        [Test]
        public void Should_set_personal_info()
        {
            var user = User.CreateNew(Address, Password, _personalInfo);

            user.PersonalInfo.Should().BeEquivalentTo(_personalInfo);
        }

        [Test]
        public void Should_throw_if_no_active_session()
        {
            var user = User.CreateNew(Address, Password, _personalInfo);

            Action sessionGet = () => user.GetActiveSession();

            sessionGet.Should().Throw<NoActiveSessionsException>();
        }

        [Test]
        public void Should_not_throw_if_session_exists()
        {
            var user = User.CreateNew(Address, Password, _personalInfo);

            user.StartNewSession(Quiz.CreateNew("", new List<VariantTask>
            {
                VariantTask.CreateNew("", "", new List<string>())
            }));

            Action sessionGet = () => user.GetActiveSession();

            sessionGet.Should().NotThrow();
        }
    }
}
