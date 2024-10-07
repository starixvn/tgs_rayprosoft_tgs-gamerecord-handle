using Tgs.GameRecord.Handle.Core.Models;

namespace Tgs.GameRecord.Handle.Core;

public interface IGameCurrencyRepository
{
	ValueTask<GameCurrencyInfo> GetAsync(string gameProviderCode, string currencyCode, decimal bet, decimal validBet, decimal settle, decimal winLoss, decimal turnover, decimal commission, decimal contribute, decimal jackpot, CancellationToken cancellationToken = default);
}
