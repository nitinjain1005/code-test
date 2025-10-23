using api.Common;
using api.Models;

namespace api.Services
{
    public class ConversionService : IConversionService
    {
        private readonly IAccountService _accountService;
        private readonly IExchangeRateService _exchangeRateService;
        private readonly ILogger<ConversionService> _logger;

        public ConversionService(
            IAccountService accountService,
            IExchangeRateService exchangeRateService,
            ILogger<ConversionService> logger)
        {
            _accountService = accountService;
            _exchangeRateService = exchangeRateService;
            _logger = logger;
        }

        public async Task<Account> GetConvertedAccount(string currency)
        {
            if (string.IsNullOrWhiteSpace(currency))
            {
                const string msg = "Currency must be provided.";
                _logger.LogWarning(msg);
                throw new ArgumentException(msg, nameof(currency));
            }

            var account = await _accountService.GetAccount()
                ?? throw new InvalidOperationException("Account service returned null.");

            // No conversion needed
            if (string.Equals(account.Currency, currency, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation("No conversion needed. Account already in {Currency}.", currency);
                return account;
            }

            var exchangeRate = await _exchangeRateService.GetExchangeRate()
                ?? throw new InvalidOperationException("Exchange rate service returned null.");

            var target = exchangeRate.Currencies?
                .FirstOrDefault(c => string.Equals(c.Name, currency, StringComparison.OrdinalIgnoreCase))
                ?? throw new ArgumentException($"Currency '{currency}' not found in exchange rates.");

            var convertedAccount = new Account
            {
                AccountNumber = account.AccountNumber,
                Currency = currency,
                Balance = Math.Round(account.Balance * target.ExchangeRate, 2),
                Transactions = account.Transactions?
                    .Select(t => new Transaction
                    {
                        Date = t.Date,
                        Balance = Math.Round(t.Balance * target.ExchangeRate, 2)
                    })
                    .ToList() ?? new List<Transaction>()
            };

            _logger.LogInformation(
                "Successfully converted account from {FromCurrency} to {ToCurrency} with rate {Rate}.",
                account.Currency, currency, target.ExchangeRate);

            return convertedAccount;


        }
    }
}
