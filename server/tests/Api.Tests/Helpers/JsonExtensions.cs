using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Api.Tests.Helpers
{
    public static class JsonExtensions
    {
        public static StringContent ToJsonContent<T>(this T value)
        {
            return new StringContent(
                JsonConvert.SerializeObject(value),
                Encoding.UTF8,
                "application/json");
        }

        public static async Task<T> GetJsonAsync<T>(this HttpResponseMessage response)
        {
            return
                JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
        }
    }
}
