using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Api.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;
using server.core;
using server.core.Api.Dto;
using server.core.Domain.Tasks;

namespace Api.Tests
{
    [TestFixture]
    public class QuizTests
    {
        private const string QuizName = "TestQuiz";
        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            var factory = new LocalWebApplicationFactory();
            var client = factory.CreateClient();
            await AuthenticateAdminAsync(client);

            var taskRequest = new CreateTaskRequest
            {
                Answer = "42",
                Question = "Meaning of life",
                Variants = new List<string> {"foo", "bar", "42"}
            };

            _firstTask = await client.CreateTaskAsync(taskRequest);
            _secondTask = await client.CreateTaskAsync(taskRequest);

            var quizRequest = new CreateQuizRequest
            {
                QuizName = QuizName,
                Tasks = new List<Guid> {_firstTask, _secondTask}
            };

            _firstQuiz = await client.CreateQuizAsync(quizRequest);
            _secondQuiz = await client.CreateQuizAsync(quizRequest);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _factory.Dispose();
        }

        [SetUp]
        public void SetUp()
        {
            _factory = new LocalWebApplicationFactory();
            Client = _factory.CreateClient();
        }

        [Test]
        public async Task Should_find_existing_quiz()
        {
            await AuthenticateAdminAsync(Client);
            var getQuiz = await Client.GetAsync(GetQuizRoute(_firstQuiz));
            var response = await getQuiz.GetJsonAsync<GetQuizResponse>();

            getQuiz.StatusCode.Should().Be(200);
            response.QuizName.Should().BeEquivalentTo(QuizName);
            response.QuizId.Should().Be(_firstQuiz);
            response.Tasks.Count.Should().Be(2);
        }

        [Test]
        public async Task Should_find_all_quizzes()
        {
            await AuthenticateAdminAsync(Client);
            var getQuizzes = await Client.GetAsync(GetAllRoute);
            var quizzes = await getQuizzes.GetJsonAsync<AllQuizzesResponse>();

            getQuizzes.StatusCode.Should().Be(200);
            quizzes.Quizzes.Count.Should().Be(2);
            quizzes.Quizzes[0].QuizId.Should().Be(_firstQuiz);
            quizzes.Quizzes[1].QuizId.Should().Be(_secondQuiz);
        }

        [Test]
        public async Task Should_get_401_when_not_authorized()
        {
            var responses = await Task.WhenAll(
                Client.GetAsync(GetAllRoute),
                Client.GetAsync(GetQuizRoute(Guid.NewGuid())),
                Client.PostAsync(CreateQuizRoute, new CreateQuizRequest().ToJsonContent()));

            foreach (var httpResponseMessage in responses)
            {
                httpResponseMessage.StatusCode.Should().Be(401);
            }
        }

        [Test]
        public async Task Should_get_403_when_not_admin()
        {
            var login = "johndoe";
            var password = "qwerty";

            await Client.RegisterAsync(login, password);

            var token = await Client.AuthenticateAsync(login, password);
            Client.SetJwt(token);

            var responses = await Task.WhenAll(
                Client.GetAsync(GetQuizRoute(Guid.NewGuid())),
                Client.PostAsync(CreateQuizRoute, new CreateQuizRequest().ToJsonContent()));

            foreach (var httpResponseMessage in responses)
            {
                httpResponseMessage.StatusCode.Should().Be(403);
            }
        }

        [Test]
        public async Task Should_get_all_quizzes_if_not_admin()
        {
            var login = "foofoo";
            var password = "barbar";

            await Client.RegisterAsync(login, password);
            var token = await Client.AuthenticateAsync(login, password);
            Client.SetJwt(token);

            var allTestsResponse = await Client.GetAsync(GetAllRoute);
            var allQuizzes = await allTestsResponse.GetJsonAsync<AllQuizzesResponse>();

            allTestsResponse.StatusCode.Should().Be(200);
            allQuizzes.Quizzes.Count.Should().Be(2);
        }


        private Guid _firstQuiz;
        private Guid _secondQuiz;
        private Guid _firstTask;
        private Guid _secondTask;

        private const string BaseRoute = "api/v1/quiz";
        private const string GetAllRoute = BaseRoute + "/all";
        private string GetQuizRoute(Guid id) => $"{BaseRoute}/{id.ToString()}";
        private const string CreateQuizRoute = BaseRoute + "/new";
        private HttpClient Client { get; set; }
        private WebApplicationFactory<Startup> _factory;

        private async Task AuthenticateAdminAsync(HttpClient client)
        {
            var response = await client.PostAsync(
                "api/auth/v1/user/authenticate",
                new AuthenticationRequest
                {
                    Email = "admin",
                    Password = "admin"
                }.ToJsonContent());

            var token = await response.Content.ReadAsStringAsync();
            client.SetJwt(token);
        }
    }
}
