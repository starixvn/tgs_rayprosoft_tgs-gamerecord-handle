using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Handle.Core.ApplicationServices;

internal class GameBetsRapidTestWorkerCommandService(
	IGameProviderRepository gameProviderRepository,
	IPlayerRepository playerRepository,
	IBrandGameProviderRepository brandGameProviderRepository,
	IGameRepository gameRepository,
	IGameCurrencyRepository gameCurrencyRepository,
	ICommandService<GameBetsRapidTestWorkerPublishCommand> service) : ICommandService<GameBetsRapidTestWorkerCommand>
{
	public async ValueTask ExecuteAsync(GameBetsRapidTestWorkerCommand command, CancellationToken cancellationToken = default)
	{
		var gameProviderId = await gameProviderRepository.GetIdAsync(
			code: command.GameProviderCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var playerInfo = await playerRepository.GetAsync(
			playerCode: command.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new ArgumentNullException(
				paramName: command.PlayerCode,
				message: $"This player does not exist.{new
				{
					command.GameProviderCode,
					command.PlayerCode,
					command.ExternalRoundId,
					command.ExternalBetId,
					command.ExternalGameId,
					command.ExternalCreatedOn,
					command.ExternalClosedOn
				}}");

		var gameProviderConfigurationId = await brandGameProviderRepository.GetGameProviderConfigurationIdAsync(
			brandId: playerInfo.BrandId,
			gameProviderId: gameProviderId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var gameInfo = await gameRepository.GetAsync(
			gameProviderId: gameProviderId,
			externalGameId: command.ExternalGameId,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new ArgumentNullException(
				paramName: command.ExternalGameId,
				message: $"This game does not exist.({new
				{
					command.GameProviderCode,
					command.PlayerCode,
					command.ExternalRoundId,
					command.ExternalBetId,
					command.ExternalGameId,
					command.ExternalCreatedOn,
					command.ExternalClosedOn
				}})");

		var bet = command.Bet;
		var validBet = command.ValidBet;
		var settle = 0.0m;
		var commission = 0.0m;
		var turnover = 0.0m;
		var winLoss = 0.0m;
		var jackpot = 0.0m;

		if (command.BetState is not GameBets.Common.Enums.BetState.Waiting and not GameBets.Common.Enums.BetState.Running)
		{
			settle = command.Settle;
			commission = command.Commission;
			turnover = command.Turnover;
			winLoss = command.WinLoss;
			jackpot = command.Jackpot;
		}

		var gameCurrencyInfo = await gameCurrencyRepository.GetAsync(
			gameProviderCode: gameInfo.GameProviderCode,
			currencyCode: playerInfo.CurrencyCode,
			bet: bet,
			validBet: validBet,
			settle: settle,
			commission: commission,
			turnover: turnover,
			winLoss: winLoss,
			jackpot: jackpot,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		await service.ExecuteAsync(
		   command: new GameBetsRapidTestWorkerPublishCommand(
			GameProviderId: gameProviderId,
			GameProviderCode: command.GameProviderCode,
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
			BetState: command.BetState,
			BetType: command.BetType,
			FreeGameState: command.FreeGameState,
			GambleState: command.GambleState,
			CashoutState: command.CashoutState,
			ResettleState: command.ResettleState,
			JackpotState: command.JackpotState,
			ExternalRoundId: command.ExternalRoundId,
			ExternalBetId: command.ExternalBetId,
			ExternalGameId: command.ExternalGameId,
			ExternalCreatedOn: command.ExternalCreatedOn,
			ExternalClosedOn: command.ExternalClosedOn,
			ExternalPayoffOn: command.ExternalPayoffOn,
			ExternalResettleOn: command.ExternalResettleOn,
			CurrencyRatio: gameCurrencyInfo.CurrencyRatio,
			Bet: gameCurrencyInfo.Bet,
			ValidBet: gameCurrencyInfo.ValidBet,
			Settle: gameCurrencyInfo.Settle,
			Commission: gameCurrencyInfo.Commission,
			Turnover: gameCurrencyInfo.Turnover,
			WinLoss: gameCurrencyInfo.WinLoss,
			Jackpot: gameCurrencyInfo.Jackpot,
			AdditionalData: command.AdditionalData),
		   cancellationToken: cancellationToken)
		   .ConfigureAwait(false);
	}
}
