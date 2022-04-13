using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
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
            var result = await _client
                .GetFromJsonAsync<PagedResult<ExpenseViewModel>>("/expenses?pageNumber=4&pageSize=20");
            result.Should().NotBeNull();
            result?.Rows.Count.Should().Be(20);
            result?.CurrentPage.Should().Be(4);
        }

        [Fact]
        public async Task CreateUpdateDelete()
        {
            var expense = new ExpenseViewModel
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
            var createResponse = await _client.PostAsJsonAsync("/Expenses", expense);
            createResponse.EnsureSuccessStatusCode();

            var created = await createResponse.ReadAsAsync<ExpenseViewModel>();
            created.Should().BeEquivalentTo(expense, options => options
                .Excluding(vm => vm.RowVersion)
                .Excluding(vm => vm.IsNew)
                .Excluding(vm => vm.ID));
            created.RowVersion.Should().Be(1);
            created.IsNew.Should().BeFalse();

            var existing = await _client.GetFromJsonAsync<ExpenseViewModel>($"/Expenses/{created.ID}")
                ?? throw new Exception("Could not get existing expense");
            existing.Description = "Δοκιμαστική Εγγραφή εξόδου";
            existing.CategoryId  = 1;

            var updateResponse = await _client.PostAsJsonAsync("/Expenses", existing);
            updateResponse.EnsureSuccessStatusCode();
            var updated = await updateResponse.ReadAsAsync<ExpenseViewModel>();
            updated.Should().BeEquivalentTo(existing, options => options
                .Excluding(vm => vm.RowVersion)
                .Excluding(vm => vm.Category)
                .Excluding(vm => vm.CategoryId));
            updated.RowVersion.Should().Be(existing.RowVersion + 1);
            updated.CategoryId.Should().Be(1);
            updated.Description.Should().Be("Δοκιμαστική Εγγραφή εξόδου");

            var deleteRequest = new HttpRequestMessage
            {
                Method     = HttpMethod.Delete,
                RequestUri = new Uri("Expenses", UriKind.Relative),
                Content = new StringContent(JsonSerializer.Serialize(new
                {
                    updated.ID,
                    updated.RowVersion
                }), Encoding.Default, "application/json")
            };
            var deleteResponse = await _client.SendAsync(deleteRequest);
            deleteResponse.EnsureSuccessStatusCode();
        }
    }
}