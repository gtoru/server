using System;
using FluentAssertions;
using NUnit.Framework;
using server.core.Domain.Authentication;
using server.core.Infrastructure.Models;

namespace Infrastructure.Tests.Models
{
    [TestFixture]
    public class SessionTests
    {
        [Test]
        public void Should_convert_from_domain()
        {
            var domainSession = Session.CreateNew(Guid.NewGuid());

            var sessionModel = SessionModel.FromDomain(domainSession);

            sessionModel.SessionId.Should().Be(domainSession.Id);
            sessionModel.UserId.Should().Be(domainSession.UserId);
            sessionModel.ValidThrough.Should().Be(domainSession.ValidThrough);
        }

        [Test]
        public void Should_convert_from_model()
        {
            var sessionModel = new SessionModel
            {
                SessionId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                ValidThrough = DateTime.UtcNow
            };

            var domainSession = SessionModel.ToDomain(sessionModel);

            domainSession.Id.Should().Be(sessionModel.SessionId);
            domainSession.UserId.Should().Be(sessionModel.UserId);
            domainSession.ValidThrough.Should().Be(sessionModel.ValidThrough);
        }
    }
}
