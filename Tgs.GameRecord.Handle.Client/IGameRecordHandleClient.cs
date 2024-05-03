using System.Text.Json.Nodes;
using Tgs.GameRecord.Handle.Client.Models;
using Tgs.GameRecord.Handle.Common.Protos;

namespace Tgs.GameRecord.Handle.Client;

public interface IGameRecordHandleClient
{

	/// <summary>
	/// Requests the publish asynchronous asynchronous.
	/// </summary>
	/// <param name="source">The source.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns></returns>
	ValueTask RequestPublishAsyncAsync(GameRecordRequest source, CancellationToken cancellationToken = default);
	/// <summary>
	/// Requests the publish asynchronous asynchronous.
	/// </summary>
	/// <param name="source">The source.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns></returns>
	ValueTask RequestPublishAsyncAsync(GameRecordRunningRequest source, CancellationToken cancellationToken = default);
	/// <summary>
	/// Requests the publish asynchronous asynchronous.
	/// </summary>
	/// <param name="source">The source.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns></returns>
	ValueTask RequestPublishAsyncAsync(GameRecordClosedRequest source, CancellationToken cancellationToken = default);
}
