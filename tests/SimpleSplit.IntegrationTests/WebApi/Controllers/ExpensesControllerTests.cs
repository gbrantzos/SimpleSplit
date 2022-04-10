using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using SimpleSplit.Application.Base;
using SimpleSplit.Application.Features.Expenses;
using Xunit;

namespace SimpleSplit.IntegrationTests.WebApi.Controllers
{
    public class ExpensesControllerTests : IClassFixture<SimpleSplitWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly SimpleSplitWebApplicationFactory _factory;

        public ExpensesControllerTests(SimpleSplitWebApplicationFactory factory)
        {
            _factory = factory;
            _client  = factory.CreateClient();
        }

        [Fact]
        public async Task GetPagedResults()
        {
            var token = await _factory.GetJwtToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/expenses?pageNumber=4&pageSize=20");
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<PagedResult<ExpenseViewModel>>(jsonResponse);

            result.Should().NotBeNull();
            result?.Rows.Count.Should().Be(20);
            result?.CurrentPage.Should().Be(4);
        }

        [Fact]
        public async Task CreateUpdateDelete()
        {
            var token = await _factory.GetJwtToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = new ExpenseViewModel
            {
                ID            = 0,
                RowVersion    = 0,
                Description   = "Test expense",
                EnteredAt     = DateTime.Today,
                Amount        = 23,
                Category      = null,
                CategoryId    = 0,
                IsOwnerCharge = true,
                SharedAt      = null
            };
            var rawBody = new StringContent(JsonConvert.SerializeObject(request), Encoding.Default, "application/json");
            var rawResponse = await _client.PostAsync("/Expenses", rawBody);
            rawResponse.EnsureSuccessStatusCode();

            var responseContent = await rawResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<ExpenseViewModel>(responseContent)
                ?? throw new Exception("Could not get create expense response");

            response.Should().BeEquivalentTo(request, options => options
                .Excluding(vm => vm.RowVersion)
                .Excluding(vm => vm.IsNew)
                .Excluding(vm => vm.ID));
            response.RowVersion.Should().Be(1);
            response.IsNew.Should().BeFalse();

            // TODO Consider using the following
            // https://timdeschryver.dev/blog/how-to-test-your-csharp-web-api#testing-multiple-endpoints-at-once-parameterized-xunit-tests
            var existingRequest = await _client.GetAsync($"/Expenses/{response.ID}");
            existingRequest.EnsureSuccessStatusCode();
            var existing =
                JsonConvert.DeserializeObject<ExpenseViewModel>(await existingRequest.Content.ReadAsStringAsync())
                ?? throw new Exception("Could not get existing expense");

            existing.Description = "Δοκιμαστική Εγγραφή εξόδου";
            existing.CategoryId  = 1;

            var updateResponse = await _client.PostAsync("/Expenses", new StringContent(
                JsonConvert.SerializeObject(existing),
                Encoding.Default,
                "application/json"));
            updateResponse.EnsureSuccessStatusCode();
            var updateResult =
                JsonConvert.DeserializeObject<ExpenseViewModel>(await updateResponse.Content.ReadAsStringAsync())
                ?? throw new Exception("Could not get updated expense");
            updateResult.Should().BeEquivalentTo(existing, options => options
                .Excluding(vm => vm.RowVersion)
                .Excluding(vm => vm.Category)
                .Excluding(vm => vm.CategoryId));
            updateResult.RowVersion.Should().Be(existing.RowVersion + 1);
            updateResult.CategoryId.Should().Be(1);
            updateResult.Description.Should().Be("Δοκιμαστική Εγγραφή εξόδου");

            var deleteRequest = new HttpRequestMessage
            {
                Method     = HttpMethod.Delete,
                RequestUri = new Uri("Expenses", UriKind.Relative),
                Content = new StringContent(JsonConvert.SerializeObject(new
                {
                    updateResult.ID,
                    updateResult.RowVersion
                }), Encoding.Default, "application/json")
            };
            var deleteResponse = await _client.SendAsync(deleteRequest);
            deleteResponse.EnsureSuccessStatusCode();
        }
    }
}