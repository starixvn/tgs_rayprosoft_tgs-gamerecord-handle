using Tgs.GameRecord.Handle.Core.Models;

namespace Tgs.GameRecord.Handle.Core;

public interface IGameCurrencyRepository
{
	public ValueTask<GameCurrencyInfo> GetAsync(string gameProviderCode, string currencyCode, decimal bet, decimal validBet, decimal settle, decimal commission, decimal turnover, decimal winLoss, decimal jackpot, CancellationToken cancellationToken = default);
}
