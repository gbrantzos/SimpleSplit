using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using SimpleSplit.Application.Features.Security;

namespace SimpleSplit.IntegrationTests
{
    public class SimpleSplitWebApplicationFactory : WebApplicationFactory<Program>
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

        public async Task<string> GetJwtToken()
        {
            // TODO Check the following
            // https://bartwullems.blogspot.com/2019/09/xunit-async-lifetime.html
            if (!String.IsNullOrEmpty(_token))
                return _token;

            var internalAdmin = Services.GetRequiredService<InternalAdministrator>();
            var request = new LoginUser
            {
                UserName = internalAdmin.UserName,
                Password = internalAdmin.Password
            };
            var rawBody = new StringContent(JsonConvert.SerializeObject(request), Encoding.Default, "application/json");
            var response = await CreateClient().PostAsync("/users", rawBody);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var loginResponse = JsonConvert.DeserializeObject<LoginUserResponse>(responseContent)
                ?? throw new Exception("Could not get user login response");

            _token = loginResponse.Token;
            return _token;
        }
    }
}