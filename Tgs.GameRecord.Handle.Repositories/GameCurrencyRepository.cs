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
        var request = new ConvertPlatformRoundsRequest(
            Bet: bet,
            ValidBet: validBet,
            Settle: settle,
            Commission: commission,
            Turnover: turnover,
            WinLoss: playerWinLoss);

        var result = await _client
            .ConvertPlatformRoundsAsync(
            gameProviderCode: gameProviderCode,
            currencyCode: currencyCode,
            request: request,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return new GameCurrencyModel(
            ExchangeRate: result.ExchangeRate,
            Bet: result.Bet,
            ValidBet: result.ValidBet,
            Settle: result.Settle,
            Commission: result.Commission,
            Turnover: result.Turnover,
            PlayerWinLoss: result.WinLoss);
    }
}
