using System.Text.Json.Nodes;
using Tgs.GameRecord.Handle.Client.Models;

namespace Tgs.GameRecord.Handle.Client;

public interface IGameRecordHandleClient
{
	/// <summary>
	/// Requests the publish asynchronous asynchronous.
	/// </summary>
	/// <param name="source">The source.</param>
	/// <param name="additionalData">The additional data.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns></returns>
	ValueTask RequestPublishAsyncAsync(GameRecordRequest source, JsonNode additionalData, CancellationToken cancellationToken = default);

	/// <summary>
	/// Requests the publish asynchronous asynchronous.
	/// </summary>
	/// <param name="source">The source.</param>
	/// <param name="additionalData">The additional data.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns></returns>
	ValueTask RequestPublishAsyncAsync(GameRecordRunningRequest source, JsonNode additionalData, CancellationToken cancellationToken = default);

	/// <summary>
	/// Requests the publish asynchronous asynchronous.
	/// </summary>
	/// <param name="source">The source.</param>
	/// <param name="additionalData">The additional data.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns></returns>
	ValueTask RequestPublishAsyncAsync(GameRecordClosedRequest source, JsonNode additionalData, CancellationToken cancellationToken = default);
}
