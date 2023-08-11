using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.GameRecord.Handle.Core;
using Tgs.GameRecord.Handle.Core.ApplicationServices;
using Tgs.MessageQueue;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Handle.Messages;

public class GameRecordMessageHandler : IMessageHandler
{
    private readonly ILogger<GameRecordMessageHandler> _logger;
    private readonly ICommandService<GameRecordHandleCommand> _service;

    public GameRecordMessageHandler(
        ILogger<GameRecordMessageHandler> logger,
        ICommandService<GameRecordHandleCommand> service)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public ValueTask HandleAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        var request = GameRecordHandleRequest.Parser.ParseFrom(data.Span)
            ?? throw new NullReferenceException("The request data resolves to a null value");

        _logger.LogInformation("LogOn:{logOn} Information Request:{request}", DateTime.Now.ToString("o"), request);

        var command = new GameRecordHandleCommand(
            GameProviderCode: request.GameProviderCode,
            ExternalRoundId: request.ExternalRoundId,
            ExternalGameId: request.ExternalGameId,
            PlayerCode: request.PlayerCode,
            Status: request.Status,
            ClientStatus: request.ClientStatus,
            ExternalCreatedOn: request.ExternalCreatedOn.ToDateTimeOffset(),
            ExternalClosedOn: request.ExternalClosedOn.ToDateTimeOffset(),
            Bet: request.Bet.ToDecimal().GetValueOrDefault(0),
            ValidBet: request.ValidBet.ToDecimal().GetValueOrDefault(0),
            Settle: request.Settle.ToDecimal().GetValueOrDefault(0),
            Commission: request.Commission.ToDecimal().GetValueOrDefault(0),
            Turnover: request.Turnover.ToDecimal().GetValueOrDefault(0),
            PlayerWinLoss: request.PlayerWinLoss.ToDecimal().GetValueOrDefault(0),
            AdditionalData: request.AdditionalData);

        return _service
            .ExecuteAsync(
            command: command,
            cancellationToken: cancellationToken);
    }
}
