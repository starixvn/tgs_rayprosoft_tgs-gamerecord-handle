using Tgs.GameRecord.Handle.Common.Protos;

namespace Tgs.GameRecord.Handle.Core.ApplicationServices;

public record GameWagerInfoResult(
    Guid GameProviderId,
    string GameProviderCode,
    Guid GameProviderConfigurationId,
    string SubGameProviderCode,
    int GameId,
    GameCategories GameCategory,
    GameTypes GameType,
    Guid ResellerId,
    string ResellerCode,
    Guid LicenseeId,
    string LicenseeCode,
    Guid BrandId,
    string BrandCode,
    string PlayerCode,
    string Username,
    string CurrencyCode,
    Guid WalletId,
    decimal ExchangeRate,
    decimal Bet,
    decimal ValidBet,
    decimal Settle,
    decimal Commission,
    decimal Turnover,
    decimal WinLoss);
