using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Api.Tests.Helpers;
using FluentAssertions;
using NUnit.Framework;
using server.core.Api.Dto;

namespace Api.Tests
{
    [TestFixture]
    public class TestSessionTests
    {
        [SetUp]
        public void SetUp()
        {
            var factory = new LocalWebApplicationFactory();
            _client = factory.CreateClient();
        }

        private HttpClient _client;

        private async Task<Guid> CreateNewQuizAsync()
        {
            var adminToken = await _client.AuthenticateAsync("admin", "admin");
            _client.SetJwt(adminToken);
            var firstId = await _client.CreateTaskAsync(new CreateTaskRequest
            {
                Answer = "42",
                Question = "abc",
                Weight = 2,
                Variants = new List<string>()
            });
            var secondId = await _client.CreateTaskAsync(new CreateTaskRequest
            {
                Answer = "42",
                Question = "def",
                Weight = 2,
                Variants = new List<string>()
            });

            var quizId = await _client.CreateQuizAsync(new CreateQuizRequest
            {
                Tasks = new List<Guid> {firstId, secondId},
                QuizName = "test quiz"
            });

            return quizId;
        }

        [Test]
        public async Task Should_fail_to_start_new_session_if_already_has_active_session()
        {
            var email = "user1";
            var password = "password";
            var quizId = await CreateNewQuizAsync();

            await _client.RegisterAsync(email, password);
            var token = await _client.AuthenticateAsync(email, password);
            _client.SetJwt(token);
            var userId = await _client.GetUserIdAsync(token);

            await _client.PostAsync(
                $"api/v1/user/{userId}/sessions/new",
                new NewTestSessionRequest
                {
                    QuizId = quizId
                }.ToJsonContent());

            var secondSession = await _client.PostAsync(
                $"api/v1/user/{userId}/sessions/new",
                new NewTestSessionRequest
                {
                    QuizId = quizId
                }.ToJsonContent());

            secondSession.StatusCode.Should().Be(409);
        }

        [Test]
        public async Task Should_not_return_current_session_if_session_ended()
        {
            var email = "user2";
            var password = "password";
            var quizId = await CreateNewQuizAsync();

            await _client.RegisterAsync(email, password);
            var token = await _client.AuthenticateAsync(email, password);
            _client.SetJwt(token);
            var userId = await _client.GetUserIdAsync(token);

            var sessionStart = await _client.PostAsync(
                $"api/v1/user/{userId}/sessions/new",
                new NewTestSessionRequest
                {
                    QuizId = quizId
                }.ToJsonContent());

            sessionStart.StatusCode.Should().Be(200);

            var sessionEnd = await _client.PostAsync(
                $"api/v1/user/{userId}/sessions/end",
                new { }.ToJsonContent());

            sessionEnd.StatusCode.Should().Be(200);

            var currentSessionGet = await _client.GetAsync(
                $"api/v1/user/{userId}/sessions/current");

            currentSessionGet.StatusCode.Should().Be(404);
        }

        [Test]
        public async Task Should_start_new_test_session()
        {
            var email = "user";
            var password = "password";
            var quizId = await CreateNewQuizAsync();

            await _client.RegisterAsync(email, password);
            var token = await _client.AuthenticateAsync(email, password);
            _client.SetJwt(token);
            var userId = await _client.GetUserIdAsync(token);

            var testSessionCreation = await _client.PostAsync(
                $"api/v1/user/{userId}/sessions/new",
                new NewTestSessionRequest
                {
                    QuizId = quizId
                }.ToJsonContent());

            testSessionCreation.StatusCode.Should().Be(200);
        }
    }
}
