using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace SimpleSplit.IntegrationTests
{
    public class UnitTest1 : IClassFixture<SimpleSplitWebApplicationFactory>
    {
        private readonly SimpleSplitWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public UnitTest1(SimpleSplitWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task Test1()
        {
            //var message = new HttpRequestMessage
            //{
            //    Method = HttpMethod.Get,
            //    RequestUri = new Uri("/weatherforecast")
            //};
            //var response = await client.SendAsync(message);
            var response = await _client.GetAsync("/weatherforecast");
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
        }
    }
}