using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SimpleSplit.IntegrationTests
{
    public static class Extensions
    {
        public static async Task<T> ReadAsAsync<T>(this HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content)
                ?? throw new Exception("Could not get instance from response");
        }
    }
}