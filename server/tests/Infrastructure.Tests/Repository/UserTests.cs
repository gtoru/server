using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using server.core.Domain;
using server.core.Infrastructure;
using server.core.Infrastructure.Contexts;
using server.core.Infrastructure.Error;

namespace Infrastructure.Tests.Repository
{
    [TestFixture]
    public class UserTests
    {
        private const string Email = "foo@bar.baz";
        private const string Password = "qwerty";

        private SqliteConnection _connection;
        private UserRepository _repository;

        [SetUp]
        public void SetUp()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<UserContext>()
                .UseSqlite(_connection)
                .Options;

            _repository = new UserRepository(() => new UserContext(options));
        }

        [TearDown]
        public void TearDown()
        {
            _connection.Close();
        }

        [Test]
        public async Task Should_not_throw_on_existing_user_search()
        {
            var user = User.CreateNew(Email, Password);

            await _repository.AddUserAsync(user);

            Func<Task> userSearch = async () => await _repository.FindUserAsync(user.UserId);

            userSearch.Should().NotThrow();
        }

        [Test]
        public void Should_throw_on_non_existent_user()
        {
            Func<Task> userSearch = async () => await _repository.FindUserAsync(Guid.NewGuid());

            userSearch.Should().Throw<UserNotFoundException>();
        }

        [Test]
        public async Task Should_find_existing_user()
        {
            var user = User.CreateNew(Email, Password);

            await _repository.AddUserAsync(user);

            var foundUser = await _repository.FindUserAsync(user.UserId);

            foundUser.UserId.Should().Be(user.UserId);
            foundUser.Email.Address.Should().BeEquivalentTo(user.Email.Address);
            foundUser.Email.IsVerified.Should().Be(user.Email.IsVerified);
            foundUser.Password.Verify(Password).Should().BeTrue();
        }

        [Test]
        public async Task Should_fail_to_add_existing_email()
        {
            var firstUser = User.CreateNew(Email, Password);
            var secondUser = User.CreateNew(Email, Password);

            await _repository.AddUserAsync(firstUser);

            Func<Task> secondUserAddition = async () => await _repository.AddUserAsync(secondUser);

            secondUserAddition.Should().Throw<UserAlreadyExistsException>();
        }
    }
}
