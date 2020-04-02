using System;
using Domain.Tests.Helpers;
using FluentAssertions;
using NUnit.Framework;
using server.core.Domain.Authentication;
using server.core.Domain.Error;

namespace Domain.Tests
{
    [TestFixture]
    public class SessionTests
    {
        [SetUp]
        public void SetUp()
        {
            _userId = Guid.NewGuid();
        }

        private Guid _userId;

        [Test]
        public void Should_be_valid_after_creation()
        {
            var session = Session.CreateNew(_userId);

            Action sessionCheck = () => session.CheckSession();
            sessionCheck.Should().NotThrow();
        }

        [Test]
        public void Should_expire_after_some_time()
        {
            var timeProvider = new FakeTimeProvider();
            timeProvider.CurrentTime = DateTime.UtcNow;

            var session = Session.CreateNew(_userId, timeProvider);
            timeProvider.CurrentTime = DateTime.MaxValue;

            Action sessionCheck = () => session.CheckSession();

            sessionCheck.Should().Throw<SessionExpiredException>();
        }

        [Test]
        public void Should_increase_valid_time_after_prolongation()
        {
            var session = Session.CreateNew(_userId);
            var validThrough = session.ValidThrough;

            session.Prolongate();
            session.ValidThrough.Should().BeAfter(validThrough);
        }

        [Test]
        public void Should_use_passed_guid_as_userid()
        {
            var session = Session.CreateNew(_userId);

            session.UserId.Should().Be(_userId);
        }
    }
}
