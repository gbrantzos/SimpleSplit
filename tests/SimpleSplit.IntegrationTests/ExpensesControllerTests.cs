using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using SimpleSplit.Application.Base;
using SimpleSplit.Application.Features.Expenses;
using Xunit;

namespace SimpleSplit.IntegrationTests
{
    public class ExpensesControllerTests : IClassFixture<SimpleSplitWebApplicationFactory>
    {
        private readonly SimpleSplitWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public ExpensesControllerTests(SimpleSplitWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_Tests()
        {
            var response = await _client.GetAsync("/expenses?pageNumber=1&pageSize=10");
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<PagedResult<ExpenseViewModel>>(jsonResponse);

            result.Should().NotBeNull();
            result?.Rows.Count.Should().Be(10);
            result?.CurrentPage.Should().Be(1);
        }
    }
}