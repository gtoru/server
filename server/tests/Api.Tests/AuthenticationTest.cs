using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Api.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NUnit.Framework;
using server.core;
using server.core.Api.Dto;

namespace Api.Tests
{
    [TestFixture]
    public class AuthenticationTest
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

        private const string BaseRoute = "api/auth/v1";
        private const string Address = "foo st.";
        private readonly DateTime _birthDay = DateTime.Now;
        private const string Employer = "G-man";
        private const string Occupation = "Alientbeater";
        private const string Name = "John Doe";
        private WebApplicationFactory<Startup> _factory;
        private const string AuthenticationRoute = BaseRoute + "/user/authenticate";
        private const string RegistrationRoute = BaseRoute + "/user/register";
        private const string SessionInfoRoute = BaseRoute + "/sessions/my";
        private HttpClient Client { get; set; }

        private async Task<HttpResponseMessage> RegisterUserAsync(string email, string password)
        {
            var registrationRequest = new RegistrationRequest
            {
                Email = email,
                Password = password,
                Address = Address,
                Birthday = _birthDay,
                Employer = Employer,
                Occupation = Occupation,
                Name = Name
            };

            return await Client.PostAsync(
                RegistrationRoute,
                registrationRequest.ToJsonContent());
        }

        private async Task<string> RegisterAndAuthenticateAsync(string email, string password)
        {
            await RegisterUserAsync(email, password);

            var authenticationRequest = new AuthenticationRequest
            {
                Email = email,
                Password = password
            };

            var authenticationResult = await Client.PostAsync(
                AuthenticationRoute,
                authenticationRequest.ToJsonContent());

            return await authenticationResult.Content.ReadAsStringAsync();
        }

        [Test]
        public async Task Should_authenticate_registered_user()
        {
            var email = "abc@gmail.com";
            var password = "123";
            await RegisterUserAsync(email, password);

            var authenticationRequest = new AuthenticationRequest
            {
                Email = email,
                Password = password
            };

            var authenticationResult = await Client.PostAsync(
                AuthenticationRoute,
                authenticationRequest.ToJsonContent());

            authenticationResult.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task Should_create_admin_user_on_startup()
        {
            var configuration = _factory.Services.GetService<IConfiguration>();
            var login = configuration["Admin:Login"];
            var password = configuration["Admin:Password"];

            var request = new AuthenticationRequest
            {
                Email = login,
                Password = password
            };

            var authenticationResult = await Client.PostAsync(
                AuthenticationRoute,
                request.ToJsonContent()
            );

            authenticationResult.StatusCode.Should().Be(200);
        }

        [Test]
        public async Task Should_get_session_info()
        {
            var email = "abc@mail.ru";
            var password = "qwertyuiop";

            var token = await RegisterAndAuthenticateAsync(email, password);
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                token);

            var sessionResponse = await Client.GetAsync(SessionInfoRoute);

            sessionResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseString = await sessionResponse.Content.ReadAsStringAsync();
            var sessionInfo = JsonConvert.DeserializeObject<SessionInfo>(responseString);

            sessionInfo.Email.Should().BeEquivalentTo(email);
            sessionInfo.PersonalInfo.Address.Should().BeEquivalentTo(Address);
            sessionInfo.PersonalInfo.Birthday.Should().BeSameDateAs(_birthDay);
            sessionInfo.PersonalInfo.Employer.Should().Be(Employer);
            sessionInfo.PersonalInfo.Name.Should().Be(Name);
            sessionInfo.PersonalInfo.Occupation.Should().Be(Occupation);
        }

        [Test]
        public async Task Should_not_authenticate_non_existing_user()
        {
            var password = "qwe";
            var request = new AuthenticationRequest
            {
                Email = "non@exist.com",
                Password = password
            };

            var response = await Client.PostAsync(
                AuthenticationRoute,
                request.ToJsonContent());

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Test]
        public async Task Should_not_authenticate_with_wrong_password()
        {
            var email = "rainbow@dash.fun";
            var password = "1337";
            var registrationRequest = new RegistrationRequest
            {
                Email = email,
                Password = password,
                Address = Address,
                Birthday = _birthDay,
                Employer = Employer,
                Occupation = Occupation,
                Name = Name
            };

            var authenticationRequest = new AuthenticationRequest
            {
                Email = email,
                Password = "123"
            };

            await Client.PostAsync(
                RegistrationRoute,
                registrationRequest.ToJsonContent());

            var authenticationResult = await Client.PostAsync(
                AuthenticationRoute,
                authenticationRequest.ToJsonContent());

            authenticationResult.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Test]
        public async Task Should_register_user()
        {
            var registrationResult = await RegisterUserAsync("testing@mail.com", "98765");

            registrationResult.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
