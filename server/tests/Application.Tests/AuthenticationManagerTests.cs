using System;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using server.core.Application;
using server.core.Domain;
using server.core.Domain.Authentication;
using server.core.Domain.Error;
using server.core.Domain.Misc;
using server.core.Domain.Storage;
using server.core.Infrastructure;
using server.core.Infrastructure.Error;

namespace Application.Tests
{
    [TestFixture]
    public class AuthenticationManagerTests
    {
        [SetUp]
        public void SetUp()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _unitOfWork.Sessions.Returns(Substitute.For<ISessionRepository>());
            _unitOfWork.Users.Returns(Substitute.For<IUserRepository>());
        }

        private const string Email = "foo@bar.baz";
        private const string Password = "qwerty";

        private IUnitOfWork _unitOfWork;

        [Test]
        public async Task Shoud_return_found_session()
        {
            var user = User.CreateNew(Email, Password);
            var session = Session.CreateNew(user.UserId);

            _unitOfWork.Users.FindUserAsync(Arg.Is(user.Email.Address)).Returns(user);
            _unitOfWork.Sessions.FindSessionByUserAsync(Arg.Is(user.UserId)).Returns(session);

            var foundSession = await AuthenticationManager.AuthenticateAsync(_unitOfWork, Email, Password);

            foundSession.Should().Be(session);
        }

        [Test]
        public async Task Should_call_add_user_when_registering()
        {
            var user = User.CreateNew(Email, Password);
            _unitOfWork.Users.AddUserAsync(Arg.Is(user)).Returns(Task.CompletedTask);

            await AuthenticationManager.RegisterUserAsync(_unitOfWork, Email, Password);
            await _unitOfWork.Users
                .Received(1)
                .AddUserAsync(Arg.Is<User>(u => u.Email.Address == Email));
        }

        [Test]
        public void Should_check_if_session_is_valid()
        {
            var timeProvider = Substitute.For<ITimeProvider>();
            timeProvider.GetCurrent().Returns(new DateTime(2000, 01, 01));

            var session = Session.CreateNew(Guid.NewGuid(), timeProvider);
            timeProvider.GetCurrent().Returns(new DateTime(2010, 01, 01));

            _unitOfWork.Sessions.FindSessionAsync(session.SessionId).Returns(session);

            Func<Task> sessionCheck = async () =>
                await AuthenticationManager.CheckSessionAsync(_unitOfWork, session.SessionId);

            sessionCheck.Should().Throw<SessionExpiredException>();
        }

        [Test]
        public async Task Should_create_new_session_if_session_not_found()
        {
            var user = User.CreateNew(Email, Password);

            _unitOfWork.Users.FindUserAsync(Arg.Is(user.Email.Address)).Returns(user);
            _unitOfWork.Sessions.FindSessionByUserAsync(Arg.Is(user.UserId)).Throws(new SessionNotFoundException());
            _unitOfWork.Sessions.AddSessionAsync(Arg.Any<Session>()).Returns(Task.CompletedTask);

            await AuthenticationManager.AuthenticateAsync(_unitOfWork, Email, Password);

            await _unitOfWork.Sessions
                .Received(1)
                .AddSessionAsync(Arg.Any<Session>());
        }

        [Test]
        public async Task Should_prolongate_found_session()
        {
            var session = Session.CreateNew(Guid.NewGuid());
            var validThrough = session.ValidThrough;

            _unitOfWork.Sessions.FindSessionAsync(session.SessionId).Returns(session);

            await AuthenticationManager.CheckSessionAsync(_unitOfWork, session.SessionId);

            session.ValidThrough.Should().BeAfter(validThrough);
        }

        [Test]
        public async Task Should_search_for_session_on_authentication()
        {
            var session = Session.CreateNew(Guid.NewGuid());
            _unitOfWork.Sessions.FindSessionAsync(session.SessionId).Returns(session);

            await AuthenticationManager.CheckSessionAsync(_unitOfWork, session.SessionId);

            await _unitOfWork.Sessions
                .Received(1)
                .FindSessionAsync(Arg.Is(session.SessionId));
        }

        [Test]
        public async Task Should_search_for_user_before_authentication()
        {
            var user = User.CreateNew(Email, Password);
            var session = Session.CreateNew(user.UserId);

            _unitOfWork.Users.FindUserAsync(Arg.Is(user.Email.Address)).Returns(user);
            _unitOfWork.Sessions.FindSessionByUserAsync(Arg.Is(user.UserId)).Returns(session);

            await AuthenticationManager.AuthenticateAsync(_unitOfWork, Email, Password);

            await _unitOfWork.Users
                .Received(1)
                .FindUserAsync(Email);
        }
    }
}
