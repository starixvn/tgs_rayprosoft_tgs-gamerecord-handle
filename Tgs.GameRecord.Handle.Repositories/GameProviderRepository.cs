using Microsoft.EntityFrameworkCore;
using Tgs.GameRecord.Handle.Core;
using Tgs.GameRecord.Handle.Repositories.Entities;

namespace Tgs.GameRecord.Handle.Repositories;

internal class GameProviderRepository(
    TgsContext context) : IGameProviderRepository
{
    public async ValueTask<Guid> GetIdAsync(string code, CancellationToken cancellationToken = default)
    {
        code = code.ToUpper();
        var gameProvider = await context.GameProviders
            .FirstOrDefaultAsync(
            predicate: gp => gp.Code == code,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NullReferenceException($"This game provider does not exist.({code})");

        return gameProvider.Id;
    }

    public IAsyncEnumerable<string> QueryByCodeAsync(CancellationToken cancellationToken = default)
        => context.GameProviders
        .Select(selector: gp => gp.Code)
        .ToAsyncEnumerable();
}
