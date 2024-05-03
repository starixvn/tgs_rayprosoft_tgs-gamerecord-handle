using Microsoft.Extensions.Caching.Memory;
using Tgs.GameRecord.Handle.Core;

namespace Tgs.GameRecord.Handle.Repositories;

internal class GameProviderCacheRepository(
	IGameProviderRepository repository,
	IMemoryCache memoryCache) : IGameProviderRepository
{
	public async ValueTask<Guid> GetIdAsync(string code, CancellationToken cancellationToken = default)
	{
		string key = string.Concat(typeof(GameProviderCacheRepository).Namespace!.Replace(".", "-").ToLower(), ":", typeof(GameProviderCacheRepository).Name!.ToLower(), ":", code);
		if (!memoryCache.TryGetValue(key, out Guid id))
		{
			id = await repository.GetIdAsync(
				code: code,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			return memoryCache.Set(
				key: key,
				value: id);
		}

		return id;
	}

	public IAsyncEnumerable<string> QueryByCodeAsync(CancellationToken cancellationToken = default)
		=> repository.QueryByCodeAsync(cancellationToken: cancellationToken);
}
