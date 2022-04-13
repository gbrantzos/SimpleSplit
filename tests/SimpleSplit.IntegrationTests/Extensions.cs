using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SimpleSplit.IntegrationTests
{
    public static class Extensions
    {
        public static async Task<T> ReadAsAsync<T>(this HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                })
                ?? throw new Exception("Could not get instance from response");
        }
    }
}