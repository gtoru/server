using System;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using server.core.Application;
using server.core.Domain;
using server.core.Domain.Storage;
using server.core.Infrastructure;
using server.core.Infrastructure.Error.AlreadyExists;

namespace Application.Tests
{
    [TestFixture]
    public class AuthenticationManagerTests
    {
        [SetUp]
        public void SetUp()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _unitOfWork.Users.Returns(Substitute.For<IUserRepository>());

            _personalInfo = new PersonalInfo(
                "John Doe",
                new DateTime(1970, 01, 01),
                "Over the Rainbow",
                "Believer",
                "Monsters inc.");
        }

        private const string Email = "foo@bar.baz";
        private const string Password = "qwerty";

        private IUnitOfWork _unitOfWork;
        private PersonalInfo _personalInfo;

        [Test]
        public async Task Should_call_add_user_when_registering()
        {
            var user = User.CreateNew(Email, Password, _personalInfo);
            _unitOfWork.Users.AddUserAsync(Arg.Is(user)).Returns(Task.CompletedTask);

            await AuthenticationManager.RegisterUserAsync(_unitOfWork, Email, Password, _personalInfo);
            await _unitOfWork.Users
                .Received(1)
                .AddUserAsync(Arg.Is<User>(u => u.Email.Address == Email));
        }

        [Test]
        public void Should_not_throw_if_admin_user_already_exists()
        {
            var admin = User.CreateAdmin(Email, Password);
            _unitOfWork.Users.AddUserAsync(Arg.Is(admin)).Throws<UserAlreadyExistsException>();

            Func<Task> adminCreation =
                async () => await AuthenticationManager.CreateAdmin(_unitOfWork, Email, Password);

            adminCreation.Should().NotThrow();
        }
    }
}
