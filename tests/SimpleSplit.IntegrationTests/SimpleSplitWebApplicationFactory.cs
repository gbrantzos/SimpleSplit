using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimpleSplit.Application.Features.Security;
using Xunit;

namespace SimpleSplit.IntegrationTests
{
    // Use IAsyncLifetime to properly retrieve a JWT token
    // https://mderriey.com/2017/09/04/async-lifetime-with-xunit/

    public class SimpleSplitWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private string? _token = null;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Modify configuration or anything else...
            base.ConfigureWebHost(builder);
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            return base.CreateHost(builder);
        }

        protected override void ConfigureClient(HttpClient client)
        {
            base.ConfigureClient(client);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        }

        #region IAsyncLifetime implementation
        public async Task InitializeAsync()
        {
            var internalAdmin = Services.GetRequiredService<InternalAdministrator>();
            var request = new LoginUser
            {
                UserName = internalAdmin.UserName,
                Password = internalAdmin.Password
            };
            var rawBody = new StringContent(JsonSerializer.Serialize(request), Encoding.Default, "application/json");
            var response = await CreateClient().PostAsync("/users", rawBody);
            response.EnsureSuccessStatusCode();

            var loginResponse = await response.ReadAsAsync<LoginUserResponse>()
                ?? throw new Exception("Could not get user login response");

            _token = loginResponse.Token;
        }

        Task IAsyncLifetime.DisposeAsync() => Task.CompletedTask;
        #endregion
    }
}