using Tgs.GameCurrency.Client;
using Tgs.GameRecord.Handle.Core;
using Tgs.GameRecord.Handle.Core.Models;

namespace Tgs.GameRecord.Handle.Repositories;

internal class GameCurrencyRepository : IGameCurrency
{
    private readonly IGameCurrencyClient _client;

    public GameCurrencyRepository(
        IGameCurrencyClient client
        )
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<GameCurrencyModel> GetAsync(
        string gameProviderCode,
        string currencyCode,
        decimal bet,
        decimal validBet,
        decimal settle,
        decimal commission,
        decimal turnover,
        decimal playerWinLoss,
        CancellationToken cancellationToken = default)
    {
        var result = await _client
            .ConvertPlatformRoundsAsync(
            gameProviderCode: gameProviderCode,
            currencyCode: currencyCode,
            bet: bet,
            validBet: validBet,
            settle: settle,
            commission: commission,
            turnover: turnover,
            winLoss: playerWinLoss,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return new GameCurrencyModel(
            CurrencyRatio: result.ExchangeRate,
            Bet: result.Bet,
            ValidBet: result.ValidBet,
            Settle: result.Settle,
            Commission: result.Commission,
            Turnover: result.Turnover,
            PlayerWinLoss: result.WinLoss);
    }
}
