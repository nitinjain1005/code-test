using api.Common;
using api.Models;
using System.Net.Http;
using System.Text.Json;

namespace api.Services
{
    public class ExchangeRateService: IExchangeRateService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ExchangeRateService> _logger;

        public ExchangeRateService(HttpClient httpClient, ILogger<ExchangeRateService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ExchangeRate> GetExchangeRate()
        {

                _logger.LogInformation("Calling ExchangeRateService API: {Url}", _httpClient.BaseAddress);
                var response = await _httpClient.GetAsync("/currencies.json");

                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    _logger.LogError("ExchangeRateService API returned {StatusCode}: {Body}", response.StatusCode, body);

                    throw new ExtServiceException(
                        $"ExchangeRate API error {(int)response.StatusCode} {response.ReasonPhrase}");
                }
                var json = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }; // for better readability

                var rates = JsonSerializer.Deserialize<ExchangeRate>(json, options);
                return rates!;
           
        }
    }
}
