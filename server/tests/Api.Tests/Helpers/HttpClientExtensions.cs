using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using server.core.Api.Dto;

namespace Api.Tests.Helpers
{
    public static class HttpClientExtensions
    {
        public static void SetJwt(this HttpClient client, string token)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                token);
        }

        public static async Task<Guid> CreateTaskAsync(this HttpClient client, CreateTaskRequest request)
        {
            return
                (await
                    (await client.PostAsync("api/v1/task/new", request.ToJsonContent()))
                    .GetJsonAsync<CreateTaskResponse>()).TaskId;
        }

        public static async Task<Guid> CreateQuizAsync(this HttpClient client, CreateQuizRequest request)
        {
            var response = await client.PostAsync("api/v1/quiz/new", request.ToJsonContent());
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<CreateQuizResponse>(content).QuizId;
        }

        public static async Task RegisterAsync(this HttpClient client, string email, string password)
        {
            await client.PostAsync("api/auth/v1/user/register", new RegistrationRequest
            {
                Address = "",
                Birthday = DateTime.UtcNow,
                Email = email,
                Employer = "",
                Name = "",
                Occupation = "",
                Password = password
            }.ToJsonContent());
        }

        public static async Task<string> AuthenticateAsync(this HttpClient client, string email, string password)
        {
            var authenticationResponse = await client.PostAsync(
                "api/auth/v1/user/authenticate",
                new AuthenticationRequest
                {
                    Email = email,
                    Password = password
                }.ToJsonContent());

            return await authenticationResponse.Content.ReadAsStringAsync();
        }

        public static async Task<Guid> GetUserIdAsync(this HttpClient client, string token)
        {
            client.SetJwt(token);
            var sessionInfoResponse = await client.GetAsync(
                "api/auth/v1/sessions/my");

            return JsonConvert.DeserializeObject<SessionInfo>(await sessionInfoResponse.Content.ReadAsStringAsync()).UserId;
        }
    }
}
