using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Tgs.GameRecord.Handle.Common;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.MessageQueue;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Handle.Core.ApplicationServices;

internal class GameRecordDataPublishCommandService : ICommandService<GameRecordDataPublishCommand>
{
    private readonly IMessageSender _sender;

    public GameRecordDataPublishCommandService(IMessageSender sender)
    {
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));
    }

    public ValueTask ExecuteAsync(GameRecordDataPublishCommand command, CancellationToken cancellationToken = default)
    {
        var data = new GameRecordPublish
        {
            GameProviderId = command.GameProviderId,
            GameProviderCode = command.GameProviderCode,
            GameProviderConfigurationId = command.GameProviderConfigurationId,
            SubGameProviderCode = command.SubGameProviderCode,
            GameId = command.GameId,
            GameType = command.GameType,
            ResellerId = command.ResellerId,
            ResellerCode = command.ResellerCode,
            LicenseeId = command.LicenseeId,
            LicenseeCode = command.LicenseeCode,
            BrandId = command.BrandId,
            BrandCode = command.BrandCode,
            PlayerCode = command.PlayerCode,
            Username = command.Username,
            CurrencyCode = command.CurrencyCode,
            WalletId = command.WalletId,
            Status = command.Status,
            ClientStatus = command.ClientStatus,
            ExternalRoundId = command.ExternalRoundId,
            ExternalGameId = command.ExternalGameId,
            ExternalCreatedOn = command.ExternalCreatedOn.ToTimestamp(),
            ExternalClosedOn = command.ExternalClosedOn.ToTimestamp(),
            ExchangeRate = command.ExchangeRate,
            Bet = command.Bet,
            ValidBet = command.ValidBet,
            Settle = command.Settle,
            Commission = command.Commission,
            Turnover = command.Turnover,
            PlayerWinLoss = command.PlayerWinLoss,
            AdditionalData = command.AdditionalData
        };
        return _sender
            .PublishAsync(
            subject: NatsConst.GameRecordDataPublish,
            data: data.ToByteArray(),
            cancellationToken: cancellationToken);
    }
}
