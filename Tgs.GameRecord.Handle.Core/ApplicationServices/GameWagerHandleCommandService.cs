using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Handle.Core.ApplicationServices;

internal class GameWagerHandleCommandService(
	TimeProvider timeProvider,
	IGameRecordDispatcher dispatcher) : ICommandService<GameWagerHandleCommand>
{
	public async ValueTask ExecuteAsync(GameWagerHandleCommand command, CancellationToken cancellationToken = default)
	{
		if (command.ExternalRoundId is null || string.IsNullOrWhiteSpace(command.ExternalRoundId))
		{
			throw new ArgumentNullException(
				paramName: command.ExternalRoundId,
				message: $"This ExternalRoundId is empty/null .({new
				{
					command.GameProviderCode,
					command.PlayerCode,
					command.ExternalRoundId,
					command.ExternalWagerId,
					command.ExternalGameId,
					command.ExternalCreatedOn,
					command.ExternalClosedOn,
					command.WagerState
				}})");
		}

		if (command.ExternalWagerId is null || string.IsNullOrWhiteSpace(command.ExternalWagerId))
		{
			throw new ArgumentNullException(
				paramName: command.ExternalWagerId,
				message: $"This ExternalWagerId is empty/null .({new
				{
					command.GameProviderCode,
					command.PlayerCode,
					command.ExternalRoundId,
					command.ExternalWagerId,
					command.ExternalGameId,
					command.ExternalCreatedOn,
					command.ExternalClosedOn,
					command.WagerState
				}})");
		}

		if (command.WagerState is not Common.Protos.WagerStates.Waiting and not Common.Protos.WagerStates.Running)
		{
			var currentTime = timeProvider.GetUtcNow();
			var externalClosedOn = command.ExternalClosedOn;
			var diff = currentTime.Subtract(externalClosedOn);
			if (diff.TotalDays > 45)
			{
				throw new ArgumentOutOfRangeException(
					paramName: command.ExternalClosedOn.ToString("u"),
					message: $"This game bet will take more than 45 days to settle.({new
					{
						command.GameProviderCode,
						command.PlayerCode,
						command.ExternalRoundId,
						command.ExternalWagerId,
						command.ExternalGameId,
						command.ExternalCreatedOn,
						command.ExternalClosedOn,
						command.WagerState,
						currentTime,
					}})");
			}
		}

		var betState = command.WagerState switch
		{
			Common.Protos.WagerStates.Running => GameBets.Common.Enums.BetState.Running,
			Common.Protos.WagerStates.Void => GameBets.Common.Enums.BetState.Void,
			Common.Protos.WagerStates.Refund => GameBets.Common.Enums.BetState.Refund,
			Common.Protos.WagerStates.Reject => GameBets.Common.Enums.BetState.Cancel,
			Common.Protos.WagerStates.Win => GameBets.Common.Enums.BetState.Win,
			Common.Protos.WagerStates.Lose => GameBets.Common.Enums.BetState.Loss,
			Common.Protos.WagerStates.Draw => GameBets.Common.Enums.BetState.Draw,
			Common.Protos.WagerStates.HalfWin => GameBets.Common.Enums.BetState.HalfWin,
			Common.Protos.WagerStates.HalfLose => GameBets.Common.Enums.BetState.HalfLoss,
			_ => GameBets.Common.Enums.BetState.Waiting
		};

		var betType = command.WagerType switch
		{
			Common.Protos.WagerTypes.Normal => GameBets.Common.Enums.BetType.Normal,
			_ => GameBets.Common.Enums.BetType.Normal
		};

		var freeGameState = command.FreeGameState switch
		{
			Common.Protos.FreeGameStates.FreeGame => GameBets.Common.Enums.FreeGameState.FreeGame,
			_ => GameBets.Common.Enums.FreeGameState.None
		};

		var gambleState = command.GambleState switch
		{
			Common.Protos.GambleStates.Gamble => GameBets.Common.Enums.GambleState.Gamble,
			_ => GameBets.Common.Enums.GambleState.None
		};

		var cashoutState = command.CashoutState switch
		{
			Common.Protos.CashoutStates.Cashout => GameBets.Common.Enums.CashoutState.Cashout,
			_ => GameBets.Common.Enums.CashoutState.None
		};

		var resettleState = command.ResettleState switch
		{
			Common.Protos.ResettleStates.Resettle => GameBets.Common.Enums.ResettleState.Resettle,
			_ => GameBets.Common.Enums.ResettleState.None
		};

		var jackpotState = GameBets.Common.Enums.JackpotState.None;

		await dispatcher.PushCommandAsync(
		   command: new GameBetsRapidTestWorkerCommand(
			   GameProviderCode: command.GameProviderCode,
			   PlayerCode: command.PlayerCode,
			   ExternalRoundId: command.ExternalRoundId,
			   ExternalBetId: command.ExternalWagerId,
			   ExternalGameId: command.ExternalGameId,
			   ExternalCreatedOn: command.ExternalCreatedOn,
			   ExternalClosedOn: command.ExternalClosedOn,
			   ExternalPayoffOn: command.ExternalPayoffOn,
			   ExternalResettleOn: command.ExternalResettleOn,
			   BetState: betState,
			   BetType: betType,
			   FreeGameState: freeGameState,
			   GambleState: gambleState,
			   CashoutState: cashoutState,
			   ResettleState: resettleState,
			   JackpotState: jackpotState,
			   Bet: command.Bet,
			   ValidBet: command.ValidBet,
			   Settle: command.Settle,
			   Commission: command.Commission,
			   Turnover: command.Turnover,
			   WinLoss: command.WinLoss,
			   Jackpot: command.Jackpot,
			   AdditionalData: command.AdditionalData),
		   cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
