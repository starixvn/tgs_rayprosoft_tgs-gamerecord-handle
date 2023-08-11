namespace Tgs.GameRecord.Handle.Core;

public interface IBrandGameProvider
{
    ValueTask<Guid> GetGameProviderConfigurationIdAsync(
        Guid brandId,
        Guid gameProviderId,
        CancellationToken cancellationToken = default);
}
