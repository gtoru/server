using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Api.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using NUnit.Framework;
using server.core;
using server.core.Api.Dto;

namespace Api.Tests
{
    [TestFixture]
    public class TaskTests
    {
        [SetUp]
        public void SetUp()
        {
            _factory = new LocalWebApplicationFactory();
            Client = _factory.CreateClient();
        }

        [TearDown]
        public void TearDown()
        {
            _factory.Dispose();
        }

        private const string BaseRoute = "api/v1/task";
        private const string GetAllRoute = BaseRoute + "/all";

        private string GetOneRoute(Guid id)
        {
            return $"{BaseRoute}/{id.ToString()}";
        }

        private const string AddTaskRoute = BaseRoute + "/new";
        private WebApplicationFactory<Startup> _factory;

        private const string Answer = "42";
        private const string Question = "Meaning of life";

        private readonly List<string> _variants = new List<string>
        {
            "Love",
            "Death",
            "42"
        };

        private HttpClient Client { get; set; }

        private async Task<string> AuthenticateAsync()
        {
            var authenticationResponse = await Client.PostAsync(
                "api/auth/v1/user/authenticate",
                new AuthenticationRequest
                {
                    Email = "admin",
                    Password = "admin"
                }.ToJsonContent());

            return await authenticationResponse.Content.ReadAsStringAsync();
        }

        private async Task<Guid> CreateTaskAsync()
        {
            var response = await Client.PostAsync(
                AddTaskRoute,
                new CreateTaskRequest
                {
                    Answer = Answer,
                    Question = Question,
                    Weight = 2,
                    Variants = _variants
                }.ToJsonContent());
            return JsonConvert
                .DeserializeObject<CreateTaskResponse>(await response.Content.ReadAsStringAsync()).TaskId;
        }

        [Test]
        public async Task Should_find_created_task()
        {
            var token = await AuthenticateAsync();
            Client.SetJwt(token);

            var createTaskResponse = await Client.PostAsync(
                AddTaskRoute,
                new CreateTaskRequest
                {
                    Answer = Answer,
                    Question = Question,
                    Weight = 2,
                    Variants = _variants
                }.ToJsonContent());
            var taskId =
                JsonConvert.DeserializeObject<CreateTaskResponse>(await createTaskResponse.Content.ReadAsStringAsync())
                    .TaskId;

            var getTaskResponse = await Client.GetAsync(
                GetOneRoute(taskId));

            getTaskResponse.StatusCode.Should().Be(200);

            var foundTask =
                JsonConvert.DeserializeObject<GetTaskResponse>(await getTaskResponse.Content.ReadAsStringAsync());

            foundTask.Question.Should().BeEquivalentTo(Question);
            foundTask.Variants.Should().BeEquivalentTo(_variants);
            foundTask.TaskId.Should().Be(taskId);
        }

        [Test]
        public async Task Should_get_200_when_on_task_add()
        {
            var token = await AuthenticateAsync();
            Client.SetJwt(token);

            var createTaskResponse = await Client.PostAsync(
                AddTaskRoute,
                new CreateTaskRequest
                {
                    Answer = Answer,
                    Question = Question,
                    Weight = 2,
                    Variants = _variants
                }.ToJsonContent());

            createTaskResponse.StatusCode.Should().Be(200);
        }

        [Test]
        public async Task Should_get_403_if_not_an_admin()
        {
            var token = await AuthenticateAsync();
            Client.SetJwt(token);
            var createdTaskId = await CreateTaskAsync();

            var registration = await Client.PostAsync(
                "api/auth/v1/user/register",
                new RegistrationRequest
                {
                    Email = "foo",
                    Password = "bar",
                    Address = "",
                    Birthday = DateTime.UtcNow,
                    Employer = "",
                    Name = "",
                    Occupation = ""
                }.ToJsonContent());

            registration.StatusCode.Should().Be(200);

            var authentication = await Client.PostAsync(
                "api/auth/v1/user/authenticate", new AuthenticationRequest
                {
                    Email = "foo",
                    Password = "bar"
                }.ToJsonContent());
            token = await authentication.Content.ReadAsStringAsync();

            Client.SetJwt(token);

            var responses = await Task.WhenAll(
                Client.PostAsync(AddTaskRoute, new CreateTaskRequest().ToJsonContent()),
                Client.GetAsync(GetAllRoute),
                Client.GetAsync(GetOneRoute(createdTaskId)));

            responses.Select(r => r.StatusCode).Should().AllBeEquivalentTo(403);
        }

        [Test]
        public async Task Should_get_all_tasks()
        {
            var token = await AuthenticateAsync();
            Client.SetJwt(token);

            await Task.WhenAll(CreateTaskAsync(), CreateTaskAsync(), CreateTaskAsync());

            var foundTasks = await Client.GetAsync(GetAllRoute);

            var response =
                JsonConvert.DeserializeObject<AllTasksResponse>(await foundTasks.Content.ReadAsStringAsync());

            response.Tasks.Count.Should().BeGreaterOrEqualTo(3);
        }

        [Test]
        public async Task Should_not_let_non_authenticated_user_to_get_or_create_tasks()
        {
            var token = await AuthenticateAsync();
            Client.SetJwt(token);
            var createdTaskId = await CreateTaskAsync();
            Client.SetJwt("");

            var responses = await Task.WhenAll(
                Client.PostAsync(AddTaskRoute, new CreateTaskRequest().ToJsonContent()),
                Client.GetAsync(GetAllRoute),
                Client.GetAsync(GetOneRoute(createdTaskId)));

            responses.Select(r => r.StatusCode).Should().AllBeEquivalentTo(401);
        }
    }
}
