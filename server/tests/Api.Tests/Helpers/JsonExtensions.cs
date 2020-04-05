using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Api.Tests.Helpers
{
    public static class JsonExtensions
    {
        public static StringContent ToJsonContent<T>(this T value)
        {
            return new StringContent(
                JsonSerializer.Serialize(value),
                Encoding.UTF8,
                "application/json");
        }
    }
}
