using MediatR;
using Tgs.GameRecord.Handle.Core.Models;

namespace Tgs.GameRecord.Handle.Core.ApplicationServices;

internal class GameRecordCommandHandler(
	TimeProvider timeProvider,
	IGameProviderRepository gameProviderRepository,
	IPlayerRepository playerRepository,
	IBrandGameProviderRepository brandGameProviderRepository,
	IGameRepository gameRepository,
	IGameCurrencyRepository gameCurrencyRepository,
	IGameBetsRapidTestWorkerPublishRepository publishRepository) : IRequestHandler<GameRecordCommandRequest>
{
	public async Task Handle(GameRecordCommandRequest request, CancellationToken cancellationToken)
	{
		if (request.ExternalRoundId is null || string.IsNullOrWhiteSpace(request.ExternalRoundId))
		{
			throw new ArgumentNullException(
				paramName: request.ExternalRoundId,
				message: $"This ExternalRoundId is empty/null .({new
				{
					request.GameProviderCode,
					request.PlayerCode,
					request.ExternalRoundId,
					request.ExternalBetId,
					request.ExternalGameId,
					request.ExternalCreatedOn,
					request.ExternalClosedOn,
					request.BetState
				}})");
		}

		if (request.ExternalBetId is null || string.IsNullOrWhiteSpace(request.ExternalBetId))
		{
			throw new ArgumentNullException(
				paramName: request.ExternalBetId,
				message: $"This ExternalBetId is empty/null .({new
				{
					request.GameProviderCode,
					request.PlayerCode,
					request.ExternalRoundId,
					request.ExternalBetId,
					request.ExternalGameId,
					request.ExternalCreatedOn,
					request.ExternalClosedOn,
					request.BetState
				}})");
		}

		var externalCreatedOn = request.ExternalCreatedOn;
		var externalClosedOn = DateTimeOffset.UnixEpoch;
		var externalPayoffOn = DateTimeOffset.UnixEpoch;
		var externalResettleOn = DateTimeOffset.UnixEpoch;
		var bet = request.Bet;
		var validBet = request.Bet;
		var settle = 0.0m;
		var winLoss = 0.0m;
		var turnover = 0.0m;
		var commission = 0.0m;
		var contribute = 0.0m;
		var jackpot = 0.0m;

		if (request.BetState is not Common.Protos.BetState.Waiting and not Common.Protos.BetState.Running)
		{
			var currentTime = timeProvider.GetUtcNow();
			externalClosedOn = request.ExternalClosedOn;
			externalPayoffOn = request.ExternalPayoffOn;
			externalResettleOn = request.ExternalResettleOn;
			validBet = request.ValidBet;
			settle = (request.Settle > 0) ? request.Settle : 0.0m;
			winLoss = request.WinLoss;
			turnover = request.Turnover;
			commission = request.Commission;
			contribute = request.Contribute;
			jackpot = request.Jackpot;

			var diff = currentTime.Subtract(externalClosedOn);
			if (diff.TotalDays > 60)
			{
				throw new ArgumentOutOfRangeException(
					paramName: request.ExternalClosedOn.ToString("u"),
					message: $"This game bet will take more than 60 days to settle.({new
					{
						request.GameProviderCode,
						request.PlayerCode,
						request.ExternalRoundId,
						request.ExternalBetId,
						request.ExternalGameId,
						request.ExternalCreatedOn,
						request.ExternalClosedOn,
						request.BetState,
						currentTime,
					}})");
			}
		}

		var gameProviderId = await gameProviderRepository.GetIdAsync(
			code: request.GameProviderCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var playerInfo = await playerRepository.GetAsync(
			playerCode: request.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new ArgumentNullException(
				paramName: request.PlayerCode,
				message: $"This player does not exist.{new
				{
					request.GameProviderCode,
					request.PlayerCode,
					request.ExternalRoundId,
					request.ExternalBetId,
					request.ExternalGameId,
					request.ExternalCreatedOn,
					request.ExternalClosedOn
				}}");

		var gameProviderConfigurationId = await brandGameProviderRepository.GetGameProviderConfigurationIdAsync(
			brandId: playerInfo.BrandId,
			gameProviderId: gameProviderId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var gameInfo = await gameRepository.GetAsync(
			gameProviderId: gameProviderId,
			externalGameId: request.ExternalGameId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new ArgumentNullException(
				paramName: request.ExternalGameId,
				message: $"This game does not exist.({new
				{
					request.GameProviderCode,
					request.PlayerCode,
					request.ExternalRoundId,
					request.ExternalBetId,
					request.ExternalGameId,
					request.ExternalCreatedOn,
					request.ExternalClosedOn
				}})");

		var betState = request.BetState switch
		{
			Common.Protos.BetState.Running => GameBets.RapidTest.Worker.Common.Protos.BetState.Running,
			Common.Protos.BetState.Void => GameBets.RapidTest.Worker.Common.Protos.BetState.Void,
			Common.Protos.BetState.Refund => GameBets.RapidTest.Worker.Common.Protos.BetState.Refund,
			Common.Protos.BetState.Cancel => GameBets.RapidTest.Worker.Common.Protos.BetState.Cancel,
			Common.Protos.BetState.Loss => GameBets.RapidTest.Worker.Common.Protos.BetState.Loss,
			Common.Protos.BetState.Win => GameBets.RapidTest.Worker.Common.Protos.BetState.Win,
			Common.Protos.BetState.Draw => GameBets.RapidTest.Worker.Common.Protos.BetState.Draw,
			Common.Protos.BetState.HalfWin => GameBets.RapidTest.Worker.Common.Protos.BetState.HalfWin,
			Common.Protos.BetState.HalfLoss => GameBets.RapidTest.Worker.Common.Protos.BetState.HalfLoss,
			_ => GameBets.RapidTest.Worker.Common.Protos.BetState.Waiting
		};

		var betType = request.BetType switch
		{
			Common.Protos.BetType.Normal => GameBets.RapidTest.Worker.Common.Protos.BetType.Normal,
			_ => GameBets.RapidTest.Worker.Common.Protos.BetType.Normal
		};

		var freeGameState = request.FreeGameState switch
		{
			Common.Protos.FreeGameState.FreeGame => GameBets.RapidTest.Worker.Common.Protos.FreeGameState.FreeGame,
			_ => GameBets.RapidTest.Worker.Common.Protos.FreeGameState.None
		};

		var gambleState = request.GambleState switch
		{
			Common.Protos.GambleState.Gamble => GameBets.RapidTest.Worker.Common.Protos.GambleState.Gamble,
			_ => GameBets.RapidTest.Worker.Common.Protos.GambleState.None
		};

		var cashoutState = request.CashoutState switch
		{
			Common.Protos.CashoutState.Cashout => GameBets.RapidTest.Worker.Common.Protos.CashoutState.Cashout,
			_ => GameBets.RapidTest.Worker.Common.Protos.CashoutState.None
		};

		var resettleState = request.ResettleState switch
		{
			Common.Protos.ResettleState.Resettle => GameBets.RapidTest.Worker.Common.Protos.ResettleState.Resettle,
			_ => GameBets.RapidTest.Worker.Common.Protos.ResettleState.None
		};

		var jackpotState = request.JackpotState switch
		{
			Common.Protos.JackpotState.Jackpot => GameBets.RapidTest.Worker.Common.Protos.JackpotState.Jackpot,
			_ => GameBets.RapidTest.Worker.Common.Protos.JackpotState.None
		};

		var gameCurrencyInfo = await gameCurrencyRepository.GetAsync(
			gameProviderCode: gameInfo.GameProviderCode,
			currencyCode: playerInfo.CurrencyCode,
			bet: bet,
			validBet: validBet,
			settle: settle,
			winLoss: winLoss,
			turnover: turnover,
			commission: commission,
			contribute: contribute,
			jackpot: jackpot,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		await publishRepository.PublishAsync(
			source: new GameBetsRapidTestWorkerPublish(
			   GameProviderId: gameProviderId,
			   GameProviderCode: request.GameProviderCode,
			   GameProviderConfigurationId: gameProviderConfigurationId,
			   SubGameProviderCode: gameInfo.SubGameProviderCode,
			   GameId: gameInfo.GameId,
			   ResellerId: playerInfo.ResellerId,
			   ResellerCode: playerInfo.ResellerCode,
			   LicenseeId: playerInfo.LicenseeId,
			   LicenseeCode: playerInfo.LicenseeCode,
			   BrandId: playerInfo.BrandId,
			   BrandCode: playerInfo.BrandCode,
			   PlayerCode: playerInfo.PlayerCode,
			   Username: playerInfo.Username,
			   CurrencyCode: playerInfo.CurrencyCode,
			   WalletId: playerInfo.WalletId,
			   ExternalRoundId: request.ExternalRoundId,
			   ExternalBetId: request.ExternalBetId,
			   ExternalGameId: request.ExternalGameId,
			   ExternalCreatedOn: externalCreatedOn,
			   ExternalClosedOn: externalClosedOn,
			   ExternalPayoffOn: externalPayoffOn,
			   ExternalResettleOn: externalResettleOn,
			   BetState: betState,
			   BetType: betType,
			   FreeGameState: freeGameState,
			   GambleState: gambleState,
			   CashoutState: cashoutState,
			   ResettleState: resettleState,
			   JackpotState: jackpotState,
			   CurrencyRatio: gameCurrencyInfo.CurrencyRatio,
			   Bet: gameCurrencyInfo.Bet,
			   ValidBet: gameCurrencyInfo.ValidBet,
			   Settle: gameCurrencyInfo.Settle,
			   WinLoss: gameCurrencyInfo.WinLoss,
			   Turnover: gameCurrencyInfo.Turnover,
			   Commission: gameCurrencyInfo.Commission,
			   Contribute: gameCurrencyInfo.Contribute,
			   Jackpot: gameCurrencyInfo.Jackpot,
			   AdditionalData: request.AdditionalData),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
