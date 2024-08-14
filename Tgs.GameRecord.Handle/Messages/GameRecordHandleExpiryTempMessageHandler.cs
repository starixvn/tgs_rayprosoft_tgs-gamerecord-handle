using System.Text.Json;
using Tgs.GameRecord.Handle.Core;
using Tgs.GameRecord.Handle.Core.ApplicationServices;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Tgs.GameRecord.Handle.Messages;

public class GameRecordHandleExpiryTempMessageHandler(
	ILogger<GameRecordHandleExpiryTempMessageHandler> logger,
	IGameRecordDispatcher dispatcher) : IMessageHandler<Common.Protos.GameRecordHandleRequest>
{
	public ValueTask HandleAsync(string subject, Common.Protos.GameRecordHandleRequest data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		logger.LogInformation("{logInformation}",
			JsonSerializer.Serialize(new
			{
				LogOn = DateTime.Now.ToString("u"),
				Subject = subject,
				Data = new
				{
					data.GameProviderCode,
					data.PlayerCode,
					data.ExternalRoundId,
					data.ExternalBetId,
					data.ExternalGameId,
					ExternalCreatedOn = data.ExternalCreatedOn.ToDateTimeOffset().ToString("u"),
					ExternalClosedOn = data.ExternalClosedOn.ToDateTimeOffset().ToString("u"),
					data.BetState
				},
				Message = "This receive message handler."
			}));

		dispatcher.ExtendExpiry(
			gameProviderCode: data.GameProviderCode,
			externalGameId: data.ExternalGameId);

		return ValueTask.CompletedTask;
	}
}
