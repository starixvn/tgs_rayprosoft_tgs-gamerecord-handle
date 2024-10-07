namespace Tgs.GameRecord.Handle.Core.Models;

public record GameCurrencyInfo(
	decimal CurrencyRatio,
	decimal Bet,
	decimal ValidBet,
	decimal Settle,
	decimal WinLoss,
	decimal Turnover,
	decimal Commission,
	decimal Contribute,
	decimal Jackpot);
