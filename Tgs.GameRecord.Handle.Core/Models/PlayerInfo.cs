namespace Tgs.GameRecord.Handle.Core.Models;

public record PlayerInfo(
    Guid ResellerId,
    string ResellerCode,
    Guid LicenseeId,
    string LicenseeCode,
    Guid BrandId,
    string BrandCode,
    string PlayerCode,
    string Username,
    Guid DefaultWalletId,
    string DefaultWalletCurrencyCode);
