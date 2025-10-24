using api.Common;
using api.Contracts;
using api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IConversionService _conversionService;
        private readonly ITransactionService _transactionService;
        private ILogger<AccountController> _logger;

        public AccountController(IConversionService conversionService, ITransactionService transactionService, ILogger<AccountController> logger)
        {
            _conversionService = conversionService;
            _transactionService = transactionService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<AccountResponse>> Get([FromQuery] string currency)
        {
            if (string.IsNullOrWhiteSpace(currency))
                return BadRequest("Parameter currency is required."); // Should be handled at central level.

            var convertedAccount = await _conversionService.GetConvertedAccount(currency);
            if (convertedAccount == null)
                return NotFound("Account details or conversion failed.");


            var (highestEarningStart, highestEarningEnd, balanceChange) =
                _transactionService.GetHighestPositiveBalanceChange(convertedAccount.Transactions);

            return Ok(new AccountResponse
            {
                AccountNumber = convertedAccount.AccountNumber,
                Balance = convertedAccount.Balance,
                Currency = convertedAccount.Currency,
                HighestBalanceChangeStart = highestEarningStart,
                HighestBalanceChangeEndDate = highestEarningEnd,
                Transactions = convertedAccount.Transactions,
                HighestBalanceChange = balanceChange
            });

        }

    }
}
