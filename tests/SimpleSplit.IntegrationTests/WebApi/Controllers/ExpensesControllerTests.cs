using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SimpleSplit.Application.Base;
using SimpleSplit.Application.Features.Expenses;
using SimpleSplit.Application.Features.Security;
using Xunit;

namespace SimpleSplit.IntegrationTests.WebApi.Controllers
{
    public class ExpensesControllerTests : IClassFixture<SimpleSplitWebApplicationFactory>
    {
        private readonly HttpClient            _client;
        private readonly InternalAdministrator _internalAdmin;
        private          string?               _token = null;

        public ExpensesControllerTests(SimpleSplitWebApplicationFactory factory)
        {
            _client        = factory.CreateClient();
            _internalAdmin = factory.Services.GetRequiredService<InternalAdministrator>();
        }

        private async Task<string> GetJwtToken()
        {
            if (!String.IsNullOrEmpty(_token))
                return _token;

            var request = new LoginUser
            {
                UserName = _internalAdmin.UserName,
                Password = _internalAdmin.Password
            };
            var rawBody = new StringContent(JsonConvert.SerializeObject(request), Encoding.Default, "application/json");
            var response = await _client.PostAsync("/users", rawBody);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var loginResponse = JsonConvert.DeserializeObject<LoginUserResponse>(responseContent)
                ?? throw new Exception("Could not get user login response");

            _token = loginResponse.Token;
            return _token;
        }

        [Fact]
        public async Task GetAll_Tests()
        {
            var token = await GetJwtToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/expenses?pageNumber=4&pageSize=20");
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<PagedResult<ExpenseViewModel>>(jsonResponse);

            result.Should().NotBeNull();
            result?.Rows.Count.Should().Be(20);
            result?.CurrentPage.Should().Be(4);
        }
    }
}