using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Tgs.GameBets.RapidTest.Worker.Client;
using Tgs.GameCurrency.Client;
using Tgs.GameCurrency.Common;
using Tgs.GameRecord.Handle.Core;

namespace Tgs.GameRecord.Handle.Repositories.UnitTest;
public class GameCurrencyRepositoryUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var faceLogger = NullLogger<GameCurrencyRepository>.Instance;
		var fakeOptions = Options.Create(new PolicySettingsOptions
		{
			RetryCount = 1,
			RetrySeconds = 1,
		});
		var fakeClient = Substitute.For<IGameCurrencyClient>();

		var gameProviderCode = "AWC_PGS";
		var currencyCode = "CNY";
		var bet = 2.0m;
		var validBet = 3.0m;
		var settle = 4.0m;
		var winLoss = 5.0m;
		var turnover = 6.0m;
		var commission = 7.0m;
		var contribute = 8.0m;
		var jackpot = 9.0m;

		var result = new ConvertPlatformRoundsResult(
			ExchangeRate: 1.0m,
			Bet: bet,
			ValidBet: validBet,
			Settle: settle,
			WinLoss: winLoss,
			Turnover: turnover,
			Commission: commission,
			Contribute: contribute,
			Jackpot: jackpot);
		_ = fakeClient.ConvertPlatformRoundsAsync(
			gameProviderCode: gameProviderCode,
			currencyCode: currencyCode,
			bet: bet,
			validBet: validBet,
			settle: settle,
			winLoss: winLoss,
			turnover: turnover,
			commission: commission,
			contribute: contribute,
			jackpot: jackpot,
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(result);

		var sut = new GameCurrencyRepository(
			faceLogger,
			fakeOptions,
			fakeClient);

		var actual = await sut.GetAsync(gameProviderCode, currencyCode, bet, validBet, settle, winLoss, turnover, commission, contribute, jackpot);

		Assert.NotNull(actual);
		Assert.Equal(result.ExchangeRate, actual.CurrencyRatio);
		Assert.Equal(result.Bet, actual.Bet);
		Assert.Equal(result.ValidBet, actual.ValidBet);
		Assert.Equal(result.Settle, actual.Settle);
		Assert.Equal(result.WinLoss, actual.WinLoss);
		Assert.Equal(result.Turnover, actual.Turnover);
		Assert.Equal(result.Commission, actual.Commission);
		Assert.Equal(result.Contribute, actual.Contribute);
		Assert.Equal(result.Jackpot, actual.Jackpot);
	}
}
