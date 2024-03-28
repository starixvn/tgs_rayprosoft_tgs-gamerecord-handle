using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.GameRecord.Handle.Core.ApplicationServices;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.Handle.Messages;

public class GameWagerMessageHandler(
    ILogger<GameWagerMessageHandler> logger,
    ICommandService<GameWagerHandleCommand> service) : IMessageHandler<GameWagerRequest>
{
    public ValueTask HandleAsync(string subject, GameWagerRequest data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
    {
        var loggerInfo = new
        {
            data.GameProviderCode,
            data.PlayerCode,
            data.WagerState,
            data.WagerType,
            data.FreeGameState,
            data.GambleState,
            data.CashoutState,
            data.ResettleState,
            data.ExternalRoundId,
            data.ExternalWagerId,
            data.ExternalGameId,
            ExternalCreatedOn = data.ExternalCreatedOn.ToDateTimeOffset(),
            ExternalClosedOn = data.ExternalClosedOn.ToDateTimeOffset(),
            ExternalPayoffOn = data.ExternalPayoffOn.ToDateTimeOffset(),
            ExternalResettleOn = data.ExternalResettleOn.ToDateTimeOffset(),
            Bet = data.Bet.ToDecimal().GetValueOrDefault(0),
            ValidBet = data.ValidBet.ToDecimal().GetValueOrDefault(0),
            Settle = data.Settle.ToDecimal().GetValueOrDefault(0),
            Commission = data.Commission.ToDecimal().GetValueOrDefault(0),
            Turnover = data.Turnover.ToDecimal().GetValueOrDefault(0),
            WinLoss = data.WinLoss.ToDecimal().GetValueOrDefault(0)
        };

        logger.LogInformation("LogOn:{logOn} Subject:{subject} Mesaage:This message has been received.({data})",
            DateTime.Now.ToString("o"),
            subject,
            loggerInfo);

        var command = new GameWagerHandleCommand(
            GameProviderCode: data.GameProviderCode,
            PlayerCode: data.PlayerCode,
            WagerState: data.WagerState,
            WagerType: data.WagerType,
            FreeGameState: data.FreeGameState,
            GambleState: data.GambleState,
            CashoutState: data.CashoutState,
            ResettleState: data.ResettleState,
            ExternalRoundId: data.ExternalRoundId,
            ExternalWagerId: data.ExternalWagerId,
            ExternalGameId: data.ExternalGameId,
            ExternalCreatedOn: data.ExternalCreatedOn.ToDateTimeOffset(),
            ExternalClosedOn: data.ExternalClosedOn.ToDateTimeOffset(),
            ExternalPayoffOn: data.ExternalPayoffOn.ToDateTimeOffset(),
            ExternalResettleOn: data.ExternalResettleOn.ToDateTimeOffset(),
            Bet: data.Bet.ToDecimal().GetValueOrDefault(0),
            ValidBet: data.ValidBet.ToDecimal().GetValueOrDefault(0),
            Settle: data.Settle.ToDecimal().GetValueOrDefault(0),
            Commission: data.Commission.ToDecimal().GetValueOrDefault(0),
            Turnover: data.Turnover.ToDecimal().GetValueOrDefault(0),
            WinLoss: data.WinLoss.ToDecimal().GetValueOrDefault(0),
            AdditionalData: data.AdditionalData);

        try
        {
            return service.ExecuteAsync(
                command: command,
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "LogOn:{logOn} Subject:{subject} Mesaage:This message has been received.({data})",
                DateTime.Now.ToString("o"),
                subject,
                new
                {
                    data.GameProviderCode,
                    data.PlayerCode,
                    data.ExternalRoundId,
                    data.ExternalWagerId,
                    data.ExternalGameId,
                    ExternalCreatedOn = data.ExternalCreatedOn.ToDateTimeOffset(),
                    ExternalClosedOn = data.ExternalClosedOn.ToDateTimeOffset(),
                    data.WagerState
                });

            throw;
        }
    }
}
