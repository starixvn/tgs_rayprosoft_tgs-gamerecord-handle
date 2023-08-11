using Tgs.GameRecord.Handle.Core.Models;

namespace Tgs.GameRecord.Handle.Core;

public interface IGameCurrency
{
    public ValueTask<GameCurrencyModel> GetAsync(string gameProviderCode, string currencyCode, decimal bet, decimal validBet, decimal settle, decimal commission, decimal turnover, decimal playerWinLoss, CancellationToken cancellationToken = default);
}
