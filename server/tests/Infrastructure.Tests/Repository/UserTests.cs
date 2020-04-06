using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using server.core.Domain;
using server.core.Domain.Authentication;
using server.core.Infrastructure;
using server.core.Infrastructure.Error;

namespace Infrastructure.Tests.Repository
{
    [TestFixture]
    public class UserTests
    {
        [SetUp]
        public void SetUp()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;

            var context = new AppDbContext(options);
            context.Database.Migrate();

            _unitOfWork = new UnitOfWork(context);

            _personalInfo = new PersonalInfo(
                "John Doe",
                new DateTime(1970, 01, 01),
                "Over the Rainbow",
                "Believer",
                "Monsters inc.");
        }

        [TearDown]
        public void TearDown()
        {
            _connection.Close();
        }

        private const string Email = "foo@bar.baz";
        private const string Password = "qwerty";

        private SqliteConnection _connection;
        private IUnitOfWork _unitOfWork;
        private PersonalInfo _personalInfo;

        [Test]
        public async Task Should_fail_to_add_existing_email()
        {
            var firstUser = User.CreateNew(Email, Password, _personalInfo);
            var secondUser = User.CreateNew(Email, Password, _personalInfo);

            await _unitOfWork.Users.AddUserAsync(firstUser);
            await _unitOfWork.SaveAsync();

            Func<Task> secondUserAddition = async () => await _unitOfWork.Users.AddUserAsync(secondUser);

            secondUserAddition.Should().Throw<UserAlreadyExistsException>();
        }

        [Test]
        public async Task Should_find_existing_user()
        {
            var user = User.CreateNew(Email, Password, _personalInfo);

            await _unitOfWork.Users.AddUserAsync(user);
            await _unitOfWork.SaveAsync();

            var foundUser = await _unitOfWork.Users.FindUserAsync(user.UserId);
            Action passwordVerification = () => foundUser.Password.Verify(Password);

            foundUser.UserId.Should().Be(user.UserId);
            foundUser.Email.Address.Should().BeEquivalentTo(user.Email.Address);
            foundUser.Email.IsVerified.Should().Be(user.Email.IsVerified);
            foundUser.PersonalInfo.Should().BeEquivalentTo(_personalInfo);
            passwordVerification.Should().NotThrow();
        }

        [Test]
        public async Task Should_find_user_by_email()
        {
            var user = User.CreateNew(Email, Password, _personalInfo);

            await _unitOfWork.Users.AddUserAsync(user);
            await _unitOfWork.SaveAsync();

            var foundUser = await _unitOfWork.Users.FindUserAsync(Email);
            Action passwordVerification = () => foundUser.Password.Verify(Password);

            foundUser.Email.Address.Should().BeEquivalentTo(Email);
            passwordVerification.Should().NotThrow();
        }

        [Test]
        public async Task Should_not_throw_on_existing_user_search()
        {
            var user = User.CreateNew(Email, Password, _personalInfo);

            await _unitOfWork.Users.AddUserAsync(user);
            await _unitOfWork.SaveAsync();

            Func<Task> userSearch = async () => await _unitOfWork.Users.FindUserAsync(user.UserId);

            userSearch.Should().NotThrow();
        }

        [Test]
        public async Task Should_save_admin_as_admin()
        {
            var admin = User.CreateAdmin(Email, Password);

            await _unitOfWork.Users.AddUserAsync(admin);
            await _unitOfWork.SaveAsync();

            var foundUser = await _unitOfWork.Users.FindUserAsync(admin.UserId);

            foundUser.AccessLevel.Should().Be(AccessLevel.Administrator);
        }

        [Test]
        public async Task Should_save_user_as_user()
        {
            var user = User.CreateNew(Email, Password, _personalInfo);

            await _unitOfWork.Users.AddUserAsync(user);
            await _unitOfWork.SaveAsync();

            var foundUser = await _unitOfWork.Users.FindUserAsync(user.UserId);

            foundUser.AccessLevel.Should().Be(AccessLevel.User);
        }

        [Test]
        public void Should_throw_on_non_existent_email()
        {
            Func<Task> userSearch = async () => await _unitOfWork.Users.FindUserAsync(Email);

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
