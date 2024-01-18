namespace Tgs.GameRecord.Handle.Core;

public interface IBrandGameProviderRepository
{
    ValueTask<Guid> GetGameProviderConfigurationIdAsync(Guid brandId, Guid gameProviderId, CancellationToken cancellationToken = default);
}
