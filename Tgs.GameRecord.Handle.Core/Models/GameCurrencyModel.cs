namespace Tgs.GameRecord.Handle.Core.Models;

public record GameCurrencyModel(
    decimal ExchangeRate,
    decimal Bet,
    decimal ValidBet,
    decimal Settle,
    decimal Commission,
    decimal Turnover,
    decimal PlayerWinLoss);
