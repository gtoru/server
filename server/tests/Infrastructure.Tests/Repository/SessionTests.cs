using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using server.core.Domain.Authentication;
using server.core.Infrastructure;
using server.core.Infrastructure.Error;

namespace Infrastructure.Tests.Repository
{
    [TestFixture]
    public class SessionTests
    {
        [SetUp]
        public void SetUp()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;

            _unitOfWork = new UnitOfWork(new AppDbContext(options));
        }

        [TearDown]
        public void TearDown()
        {
            _connection.Close();
        }

        private SqliteConnection _connection;
        private UnitOfWork _unitOfWork;

        [Test]
        public async Task Should_add_new_session_and_find_it()
        {
            var userId = Guid.NewGuid();
            var session = Session.CreateNew(userId);

            await _unitOfWork.Sessions.AddSessionAsync(session);
            await _unitOfWork.SaveAsync();

            var foundSession = await _unitOfWork.Sessions.FindSessionAsync(session.Id);

            foundSession.Id.Should().Be(session.Id);
            foundSession.UserId.Should().Be(userId);
        }

        [Test]
        public async Task Should_not_throw_on_existing_user_search()
        {
            var session = Session.CreateNew(Guid.NewGuid());

            await _unitOfWork.Sessions.AddSessionAsync(session);
            await _unitOfWork.SaveAsync();

            Func<Task> sessionSearch = async () => await _unitOfWork.Sessions.FindSessionAsync(session.Id);

            sessionSearch.Should().NotThrow();
        }

        [Test]
        public void Should_throw_when_trying_find_non_existent_session()
        {
            var sessionId = Guid.NewGuid();

            Func<Task> sessionSearch = async () => await _unitOfWork.Sessions.FindSessionAsync(sessionId);

            sessionSearch.Should().Throw<SessionNotFoundException>();
        }

        [Test]
        public async Task Should_throw_when_trying_to_add_existing_session()
        {
            var userId = Guid.NewGuid();
            var firstSession = Session.CreateNew(userId);

            await _unitOfWork.Sessions.AddSessionAsync(firstSession);
            await _unitOfWork.SaveAsync();

            var secondSession = Session.CreateNew(userId);

            Func<Task> sessionCreation = async () => await _unitOfWork.Sessions.AddSessionAsync(secondSession);

            sessionCreation.Should().Throw<SessionAlreadyExistsException>();
        }
    }
}
