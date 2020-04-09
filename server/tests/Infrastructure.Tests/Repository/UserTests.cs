using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using server.core.Domain;
using server.core.Domain.Authentication;
using server.core.Domain.Error;
using server.core.Domain.Tasks;
using server.core.Infrastructure;
using server.core.Infrastructure.Error.AlreadyExists;
using server.core.Infrastructure.Error.NotFound;

namespace Infrastructure.Tests.Repository
{
    [TestFixture]
    public class UserTests
    {
        [OneTimeSetUp]
        public async Task SetUp()
        {
            _context = await DbSetUpFixture.GetContextAsync();
            _unitOfWork = new UnitOfWork(_context);

            _personalInfo = new PersonalInfo(
                "John Doe",
                new DateTime(1970, 01, 01),
                "Over the Rainbow",
                "Believer",
                "Monsters inc.");

            _user = User.CreateNew(Email, Password, _personalInfo);

            await _unitOfWork.Users.AddUserAsync(_user);
            await _unitOfWork.SaveAsync();
        }

        private const string Email = "foo@bar.baz";
        private const string Password = "qwerty";

        private IUnitOfWork _unitOfWork;
        private PersonalInfo _personalInfo;
        private AppDbContext _context;
        private User _user;

        [Test]
        public async Task Should_create_user_with_empty_test_sessions()
        {
            var user = await _unitOfWork.Users.FindUserAsync(_user.UserId);

            user.TestSessions.Should().BeEmpty();

            Func<TestSession> sessionGet = () => user.CurrentSession;

            sessionGet.Should().Throw<NoSessionsException>();
        }

        [Test]
        public void Should_fail_to_add_existing_email()
        {
            var secondUser = User.CreateNew(Email, Password, _personalInfo);

            Func<Task> secondUserAddition = async () => await _unitOfWork.Users.AddUserAsync(secondUser);

            secondUserAddition.Should().Throw<UserAlreadyExistsException>();
        }

        [Test]
        public async Task Should_find_existing_user()
        {
            var foundUser = await _unitOfWork.Users.FindUserAsync(_user.UserId);
            Action passwordVerification = () => foundUser.Password.Verify(Password);

            foundUser.UserId.Should().Be(_user.UserId);
            foundUser.Email.Address.Should().BeEquivalentTo(_user.Email.Address);
            foundUser.Email.IsVerified.Should().Be(_user.Email.IsVerified);
            foundUser.PersonalInfo.Should().BeEquivalentTo(_personalInfo);
            passwordVerification.Should().NotThrow();
        }

        [Test]
        public async Task Should_find_user_by_email()
        {
            var foundUser = await _unitOfWork.Users.FindUserAsync(Email);
            Action passwordVerification = () => foundUser.Password.Verify(Password);

            foundUser.Email.Address.Should().BeEquivalentTo(Email);
            passwordVerification.Should().NotThrow();
        }

        [Test]
        public void Should_not_throw_on_existing_user_search()
        {
            Func<Task> userSearch = async () => await _unitOfWork.Users.FindUserAsync(_user.UserId);

            userSearch.Should().NotThrow();
        }

        [Test]
        public async Task Should_save_admin_as_admin()
        {
            var admin = User.CreateAdmin("admin", "admin");

            await _unitOfWork.Users.AddUserAsync(admin);
            await _unitOfWork.SaveAsync();

            var foundUser = await _unitOfWork.Users.FindUserAsync(admin.UserId);

            foundUser.AccessLevel.Should().Be(AccessLevel.Administrator);
        }

        [Test]
        public async Task Should_save_user_as_user()
        {
            var foundUser = await _unitOfWork.Users.FindUserAsync(_user.UserId);

            foundUser.AccessLevel.Should().Be(AccessLevel.User);
        }

        [Test]
        public void Should_throw_on_non_existent_email()
        {
            Func<Task> userSearch = async () => await _unitOfWork.Users.FindUserAsync("i@dont.exist");

            userSearch.Should().Throw<UserNotFoundException>();
        }

        [Test]
        public void Should_throw_on_non_existent_user()
        {
            Func<Task> userSearch = async () => await _unitOfWork.Users.FindUserAsync(Guid.NewGuid());

            userSearch.Should().Throw<UserNotFoundException>();
        }
    }
}
