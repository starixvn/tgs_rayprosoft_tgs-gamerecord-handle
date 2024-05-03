using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using Polly;
using Tgs.GameRecord.Handle.Core;
using Tgs.GameRecord.Handle.Core.Models;
using Tgs.GameRecord.Handle.Repositories.Entities;

namespace Tgs.GameRecord.Handle.Repositories;

internal class PlayerRepository(
	ILogger<PlayerRepository> logger,
	IOptions<PolicySettingsOptions> options,
	TgsContext context) : IPlayerRepository
{
	private readonly PolicySettingsOptions _policySettingsOptions = options.Value;

	public async ValueTask<PlayerInfo?> GetAsync(string playerCode, CancellationToken cancellationToken = default)
	{
		playerCode = playerCode.ToLower();

		var retryCount = _policySettingsOptions.RetryCount;
		var retrySeconds = _policySettingsOptions.RetrySeconds;
		var retryPolicy = Policy
			.Handle<NpgsqlException>()
			.WaitAndRetryAsync(
				retryCount: retryCount,
				retryAttempt => TimeSpan.FromSeconds(Math.Pow(retrySeconds, retryAttempt)),
				onRetry: (exception, retryTime, retryCount, context) => logger.LogWarning(exception, "LogOn:{logOn}  RetryTime:{retryTime} RetryCount:{retryCount}",
					DateTime.Now.ToString("o"),
					retryTime,
					retryCount));

		return await retryPolicy.ExecuteAsync(() => context.Players
				.Where(predicate => predicate.PlayerCode == playerCode)
				.Join(
					context.Brands,
					inner => inner.BrandId,
					outer => outer.Id,
					(player, brand) => new
					{
						brand.LicenseeId,
						BrandId = brand.Id,
						BrandCode = brand.Code,
						player.PlayerCode,
						player.Username,
						player.DefaultWalletId,
						player.DefaultWalletCurrencyCode
					})
				.Join(
					context.Licensees,
					inner => inner.LicenseeId,
					outer => outer.Id,
					(combined, licensee) => new
					{
						licensee.ResellerId,
						LicenseeId = licensee.Id,
						LicenseeCode = licensee.Code,
						combined.BrandId,
						combined.BrandCode,
						combined.PlayerCode,
						combined.Username,
						combined.DefaultWalletId,
						combined.DefaultWalletCurrencyCode
					})
				.Join(
					context.Resellers,
					inner => inner.ResellerId,
					outer => outer.Id,
					(combined, reseller) => new
					{
						ResellerId = reseller.Id,
						ResellerCode = reseller.Code,
						combined.LicenseeId,
						combined.LicenseeCode,
						combined.BrandId,
						combined.BrandCode,
						combined.PlayerCode,
						combined.Username,
						combined.DefaultWalletId,
						combined.DefaultWalletCurrencyCode
					})
				.Select(selector: player => new PlayerInfo(
					player.ResellerId,
					player.ResellerCode,
					player.LicenseeId,
					player.LicenseeCode,
					player.BrandId,
					player.BrandCode,
					player.PlayerCode,
					player.Username,
					player.DefaultWalletId,
					player.DefaultWalletCurrencyCode))
				.FirstOrDefaultAsync(
					cancellationToken: cancellationToken))
			.ConfigureAwait(false);
	}
}
