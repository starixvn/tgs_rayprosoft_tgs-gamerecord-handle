using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.GameRecord.Handle.Core.ApplicationServices;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.Handle.Messages;

public class GameWagerMessageHandler(
    ILogger<GameWagerMessageHandler> logger,
    ICommandService<GameWagerHandleCommand> service) : IMessageHandler
{
    public ValueTask HandleAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        var request = GameWagerRequest.Parser.ParseFrom(data.Span)
            ?? throw new NullReferenceException("The request data resolves to a null value");

        logger.LogInformation("LogOn:{logOn} Information Request:{request}", DateTime.Now.ToString("o"), request);

        var command = new GameWagerHandleCommand(
            GameProviderCode: request.GameProviderCode,
            PlayerCode: request.PlayerCode,
            WagerState: request.WagerState,
            WagerType: request.WagerType,
            FreeGameState: request.FreeGameState,
            GambleState: request.GambleState,
            CashoutState: request.CashoutState,
            ResettleState: request.ResettleState,
            ExternalRoundId: request.ExternalRoundId,
            ExternalWagerId: request.ExternalWagerId,
            ExternalGameId: request.ExternalGameId,
            ExternalCreatedOn: request.ExternalCreatedOn.ToDateTimeOffset(),
            ExternalClosedOn: request.ExternalClosedOn.ToDateTimeOffset(),
            ExternalPayoffOn: request.ExternalPayoffOn.ToDateTimeOffset(),
            ExternalResettleOn: request.ExternalResettleOn.ToDateTimeOffset(),
            Bet: request.Bet.ToDecimal().GetValueOrDefault(0),
            ValidBet: request.ValidBet.ToDecimal().GetValueOrDefault(0),
            Settle: request.Settle.ToDecimal().GetValueOrDefault(0),
            Commission: request.Commission.ToDecimal().GetValueOrDefault(0),
            Turnover: request.Turnover.ToDecimal().GetValueOrDefault(0),
            WinLoss: request.WinLoss.ToDecimal().GetValueOrDefault(0),
            AdditionalData: request.AdditionalData);

        return service.ExecuteAsync(
            command: command,
            cancellationToken: cancellationToken);
    }
}
