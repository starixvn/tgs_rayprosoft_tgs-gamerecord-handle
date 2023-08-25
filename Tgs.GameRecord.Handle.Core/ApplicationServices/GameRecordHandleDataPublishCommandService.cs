using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Tgs.GameRecord.Handle.Common;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.MessageQueue;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Handle.Core.ApplicationServices;

internal class GameRecordHandleDataPublishCommandService : ICommandService<GameRecordHandleDataPublishCommand>
{
    private readonly IMessageSender _sender;

    public GameRecordHandleDataPublishCommandService(IMessageSender sender)
    {
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));
    }

    public ValueTask ExecuteAsync(GameRecordHandleDataPublishCommand command, CancellationToken cancellationToken = default)
    {
        var wagerState = command.Status != GameRecordStatus.Closed
            ? WagerStates.Running
            : command.ClientStatus switch
            {
                GameRecordClientStatus.New => WagerStates.Running,
                GameRecordClientStatus.Open => WagerStates.Running,
                GameRecordClientStatus.Void => WagerStates.Void,
                GameRecordClientStatus.Win => WagerStates.Win,
                GameRecordClientStatus.Loss => WagerStates.Lose,
                GameRecordClientStatus.Draw => WagerStates.Draw,
                _ => WagerStates.Waiting
            };

        var data = new GameWagerPublish
        {
            GameProviderId = command.GameProviderId,
            GameProviderCode = command.GameProviderCode,
            GameProviderConfigurationId = command.GameProviderConfigurationId,
            SubGameProviderCode = command.SubGameProviderCode,
            GameId = command.GameId,
            GameCategory = command.GameCategory,
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
            WagerState = wagerState,
            WagerType = WagerTypes.Normal,
            ExternalRoundId = command.ExternalRoundId,
            ExternalWagerId = command.ExternalRoundId,
            ExternalGameId = command.ExternalGameId,
            ExternalCreatedOn = command.ExternalCreatedOn.ToTimestamp(),
            ExternalClosedOn = command.ExternalClosedOn.ToTimestamp(),
            ExternalPayoffOn = command.ExternalClosedOn.ToTimestamp(),
            ExchangeRate = command.ExchangeRate,
            Bet = command.Bet,
            ValidBet = command.ValidBet,
            Settle = command.Settle,
            Commission = command.Commission,
            Turnover = command.Turnover,
            PlayerWinLoss = command.PlayerWinLoss,
            AdditionalData = command.AdditionalData,
            HandleOn = DateTimeOffset.UtcNow.ToTimestamp(),
            RapidTestOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
            CreatedOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
            ClosedOn = DateTimeOffset.UnixEpoch.ToTimestamp(),
            UpdatedOn = DateTimeOffset.UtcNow.ToTimestamp()
        };

        return _sender
            .PublishAsync(
            subject: NatsConst.GameRecordHandleDataPublish,
            data: data.ToByteArray(),
            cancellationToken: cancellationToken);
    }
}
