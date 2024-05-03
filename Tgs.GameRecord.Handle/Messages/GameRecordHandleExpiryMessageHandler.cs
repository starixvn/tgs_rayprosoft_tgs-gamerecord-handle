using Tgs.GameRecord.Handle.Core;
using Valhalla.MessageQueue;

namespace Tgs.GameRecord.Handle.Messages;

public class GameRecordHandleExpiryMessageHandler(
	ILogger<GameRecordHandleExpiryMessageHandler> logger,
	IGameRecordDispatcher dispatcher) : IMessageHandler<Common.Protos.GameRecordHandleExpiryRequest>
{
	public ValueTask HandleAsync(string subject, Common.Protos.GameRecordHandleExpiryRequest data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		logger.LogInformation("LogOn:{logOn} Subject:{subject} Mesaage:This message has been received.({data})",
			DateTime.Now.ToString("o"),
			subject,
			new
			{
				data.GameProviderCode,
				data.ExternalGameId
			});

		try
		{
			dispatcher.ExtendExpiry(
				gameProviderCode: data.GameProviderCode,
				externalGameId: data.ExternalGameId);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "LogOn:{logOn} Subject:{subject} Mesaage:This message has been received.({data})",
				DateTime.Now.ToString("o"),
				subject,
				new
				{
					data.GameProviderCode,
					data.ExternalGameId
				});

			throw;
		}

		return ValueTask.CompletedTask;
	}
}
