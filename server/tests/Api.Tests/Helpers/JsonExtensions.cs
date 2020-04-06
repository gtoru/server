using System.Net.Http;
using System.Text;
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
    }
}
