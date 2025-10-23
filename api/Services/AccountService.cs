using api.Common;
using api.Models;
using System.Text.Json;

namespace api.Services
{
    public class AccountService : IAccountService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AccountService> _logger;

        public AccountService(HttpClient httpClient, ILogger<AccountService> logger)
        {
            _httpClient = httpClient;               
            _logger = logger;
        }
        public async Task<Account> GetAccount()
        {
            _logger.LogInformation("Calling external Account API: {Url}", _httpClient.BaseAddress);

                var response = await _httpClient.GetAsync("/account.json");

                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Account API returned {StatusCode}: {Body}", response.StatusCode, body);

                    throw new ExtServiceException(
                        $"Account API error {(int)response.StatusCode} {response.ReasonPhrase}");
                }

                var json = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var account = JsonSerializer.Deserialize<Account>(json, options);
                return account!;
           
        }
    }
}
