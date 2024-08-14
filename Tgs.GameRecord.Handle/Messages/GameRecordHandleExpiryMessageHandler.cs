using System.Text.Json;
using Tgs.GameRecord.Handle.Core;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.Handle.Messages;

public class GameRecordHandleExpiryMessageHandler(
	ILogger<GameRecordHandleExpiryMessageHandler> logger,
	IGameRecordDispatcher dispatcher) : IMessageHandler<Common.Protos.GameRecordHandleExpiryRequest>
{
	public ValueTask HandleAsync(string subject, Common.Protos.GameRecordHandleExpiryRequest data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		logger.LogInformation("{logInformation}",
			JsonSerializer.Serialize(new
			{
				LogOn = DateTime.Now.ToString("u"),
				Subject = subject,
				Data = new
				{
					data.GameProviderCode,
					data.ExternalGameId
				},
				Message = "This receive message handler."
			}));

		dispatcher.ExtendExpiry(
			gameProviderCode: data.GameProviderCode,
			externalGameId: data.ExternalGameId);

		return ValueTask.CompletedTask;
	}
}
