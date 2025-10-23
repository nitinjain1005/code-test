using api.Models;

namespace api.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ILogger<TransactionService> _logger;

        public TransactionService(ILogger<TransactionService> logger)
        {
            _logger = logger;
        }

        public (DateTime? start, DateTime? end, decimal highestBalanceChange) GetHighestPositiveBalanceChange(List<Transaction> transactions)
        {
            if (transactions == null || transactions.Count == 0)
            {
                _logger.LogWarning("Transaction list is null or empty.");
                return (null, null, 0);
            }
            var ordered = transactions.OrderBy(t => t.Date).ToList();

            decimal maxChange = 0;
            DateTime? bestStart = null;
            DateTime? bestEnd = null;

            decimal currentStartBalance = ordered[0].Balance;
            DateTime currentStartDate = ordered[0].Date;

            for (int i = 1; i < ordered.Count; i++)
            {
                var tx = ordered[i];

                var change = tx.Balance - currentStartBalance;

                if (change > maxChange)
                {
                    maxChange = change;
                    bestStart = currentStartDate;
                    bestEnd = tx.Date;
                }

                // if balance dropped, start new potential range
                if (tx.Balance < ordered[i - 1].Balance)
                {
                    currentStartBalance = tx.Balance;
                    currentStartDate = tx.Date;
                }
            }

            if (maxChange <= 0)
            {
                _logger.LogWarning("No positive balance change found in transactions.");
                return (null, null, 0);
            }
            _logger.LogWarning("Highest positive balance change found: {Change} from {Start} to {End}",
                                   maxChange, bestStart, bestEnd);
            return (bestStart, bestEnd, maxChange);
        }
    }
}
