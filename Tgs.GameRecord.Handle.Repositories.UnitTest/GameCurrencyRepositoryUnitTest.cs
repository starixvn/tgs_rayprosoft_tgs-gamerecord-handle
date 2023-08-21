using NSubstitute;
using Tgs.GameCurrency.Client;

namespace Tgs.GameRecord.Handle.Repositories.UnitTest;

public class GameCurrencyRepositoryUnitTest
{
    [Fact]
    public async Task ExchangeRate_1()
    {
        var fakeClient = Substitute.For<IGameCurrencyClient>();
        var gameProviderCode = "KY";
        var currencyCode = "CNY";
        var bet = 4m;
        var validBet = 4m;
        var settle = 8m;
        var commission = 0m;
        var turnover = 4m;
        var playerWinLoss = 4m;

        var result = new ConvertPlatformRoundsResult(
            ExchangeRate: 1,
            Bet: 4,
            ValidBet: 4,
            Settle: 8,
            Commission: 0,
            Turnover: 4,
            WinLoss: 4);
        _ = fakeClient
            .ConvertPlatformRoundsAsync(
            gameProviderCode: Arg.Is(gameProviderCode),
            currencyCode: Arg.Is(currencyCode),
            bet: Arg.Is(bet),
            validBet: Arg.Is(validBet),
            settle: Arg.Is(settle),
            commission: Arg.Is(commission),
            turnover: Arg.Is(turnover),
            winLoss: Arg.Is(playerWinLoss),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(result);

        var service = new GameCurrencyRepository(
            fakeClient);

        var actual = await service
            .GetAsync(
            gameProviderCode: gameProviderCode,
            currencyCode: currencyCode,
            bet: bet,
            validBet: validBet,
            settle: settle,
            commission: commission,
            turnover: turnover,
            playerWinLoss: playerWinLoss)
            .ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.Equal(1, actual.ExchangeRate);
        Assert.Equal(4m, actual.Bet);
        Assert.Equal(4m, actual.ValidBet);
        Assert.Equal(8m, actual.Settle);
        Assert.Equal(0m, actual.Commission);
        Assert.Equal(4m, actual.Turnover);
        Assert.Equal(4m, actual.PlayerWinLoss);
    }

    [Fact]
    public async Task ExchangeRate_1000()
    {
        var fakeClient = Substitute.For<IGameCurrencyClient>();

        var gameProviderCode = "KY";
        var currencyCode = "CNY";
        var bet = 20m;
        var validBet = 8m;
        var settle = 8m;
        var commission = 0m;
        var turnover = 8m;
        var playerWinLoss = 12m;

        var result = new ConvertPlatformRoundsResult(
            ExchangeRate: 1000m,
            Bet: 20000m,
            ValidBet: 8000m,
            Settle: 8000m,
            Commission: 0,
            Turnover: 8000m,
            WinLoss: 12000m);
        _ = fakeClient
            .ConvertPlatformRoundsAsync(
            gameProviderCode: Arg.Is(gameProviderCode),
            currencyCode: Arg.Is(currencyCode),
            bet: Arg.Is(bet),
            validBet: Arg.Is(validBet),
            settle: Arg.Is(settle),
            commission: Arg.Is(commission),
            turnover: Arg.Is(turnover),
            winLoss: Arg.Is(playerWinLoss),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(result);

        var service = new GameCurrencyRepository(
            fakeClient);

        var actual = await service
            .GetAsync(
            gameProviderCode: gameProviderCode,
            currencyCode: currencyCode,
            bet: bet,
            validBet: validBet,
            settle: settle,
            commission: commission,
            turnover: turnover,
            playerWinLoss: playerWinLoss)
            .ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.Equal(1000m, actual.ExchangeRate);
        Assert.Equal(20000m, actual.Bet);
        Assert.Equal(8000m, actual.ValidBet);
        Assert.Equal(8000m, actual.Settle);
        Assert.Equal(0m, actual.Commission);
        Assert.Equal(8000m, actual.Turnover);
        Assert.Equal(12000m, actual.PlayerWinLoss);
    }
}
