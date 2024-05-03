using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Handle.Core.ApplicationServices;

internal class GameRecordHandleCommandService(
	TimeProvider timeProvider,
	IGameRecordDispatcher dispatcher) : ICommandService<GameRecordHandleCommand>
{
	public async ValueTask ExecuteAsync(GameRecordHandleCommand command, CancellationToken cancellationToken = default)
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
					command.ExternalBetId,
					command.ExternalGameId,
					command.ExternalCreatedOn,
					command.ExternalClosedOn,
					command.BetState
				}})");
		}

		if (command.ExternalBetId is null || string.IsNullOrWhiteSpace(command.ExternalBetId))
		{
			throw new ArgumentNullException(
				paramName: command.ExternalBetId,
				message: $"This ExternalBetId is empty/null .({new
				{
					command.GameProviderCode,
					command.PlayerCode,
					command.ExternalRoundId,
					command.ExternalBetId,
					command.ExternalGameId,
					command.ExternalCreatedOn,
					command.ExternalClosedOn,
					command.BetState
				}})");
		}

		if (command.BetState is not Common.Protos.BetState.Waiting and not Common.Protos.BetState.Running)
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
						command.ExternalBetId,
						command.ExternalGameId,
						command.ExternalCreatedOn,
						command.ExternalClosedOn,
						command.BetState,
						currentTime,
					}})");
			}
		}

		var betState = command.BetState switch
		{
			Common.Protos.BetState.Running => GameBets.Common.Enums.BetState.Running,
			Common.Protos.BetState.Void => GameBets.Common.Enums.BetState.Void,
			Common.Protos.BetState.Refund => GameBets.Common.Enums.BetState.Refund,
			Common.Protos.BetState.Cancel => GameBets.Common.Enums.BetState.Cancel,
			Common.Protos.BetState.Win => GameBets.Common.Enums.BetState.Win,
			Common.Protos.BetState.Loss => GameBets.Common.Enums.BetState.Loss,
			Common.Protos.BetState.Draw => GameBets.Common.Enums.BetState.Draw,
			Common.Protos.BetState.HalfWin => GameBets.Common.Enums.BetState.HalfWin,
			Common.Protos.BetState.HalfLoss => GameBets.Common.Enums.BetState.HalfLoss,
			_ => GameBets.Common.Enums.BetState.Waiting
		};

		var betType = command.BetType switch
		{
			Common.Protos.BetType.Normal => GameBets.Common.Enums.BetType.Normal,
			_ => GameBets.Common.Enums.BetType.Normal
		};

		var freeGameState = command.FreeGameState switch
		{
			Common.Protos.FreeGameState.FreeGame => GameBets.Common.Enums.FreeGameState.FreeGame,
			_ => GameBets.Common.Enums.FreeGameState.None
		};

		var gambleState = command.GambleState switch
		{
			Common.Protos.GambleState.Gamble => GameBets.Common.Enums.GambleState.Gamble,
			_ => GameBets.Common.Enums.GambleState.None
		};

		var cashoutState = command.CashoutState switch
		{
			Common.Protos.CashoutState.Cashout => GameBets.Common.Enums.CashoutState.Cashout,
			_ => GameBets.Common.Enums.CashoutState.None
		};

		var resettleState = command.ResettleState switch
		{
			Common.Protos.ResettleState.Resettle => GameBets.Common.Enums.ResettleState.Resettle,
			_ => GameBets.Common.Enums.ResettleState.None
		};

		var jackpotState = command.JackpotState switch
		{
			Common.Protos.JackpotState.Jackpot => GameBets.Common.Enums.JackpotState.Jackpot,
			_ => GameBets.Common.Enums.JackpotState.None
		};

		await dispatcher.PushCommandAsync(
		   command: new GameBetsRapidTestWorkerCommand(
			   GameProviderCode: command.GameProviderCode,
			   PlayerCode: command.PlayerCode,
			   ExternalRoundId: command.ExternalRoundId,
			   ExternalBetId: command.ExternalBetId,
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
