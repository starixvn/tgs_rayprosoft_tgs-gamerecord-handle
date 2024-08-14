using System.Net.Http.Json;
using System.Text.Json;

using System.Text.Json.Serialization;

namespace Tgs.GameRecord.Handle.Client;

internal class GameRecordHandleClient(
	HttpClient httpClient) : IGameRecordHandleClient
{
	private static readonly JsonSerializerOptions _SerializerOptions = new()
	{
		WriteIndented = false,
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		Converters = { new JsonStringEnumConverter() }
	};

	public async ValueTask HandleAsync(Common.ViewModels.GameRecordHandleViewModel source, CancellationToken cancellationToken = default)
	{
		var requestContent = JsonContent.Create(source, options: _SerializerOptions);

		var httpResponseMessage = await httpClient.PostAsync(
			"/handle",
			requestContent,
			cancellationToken)
			.ConfigureAwait(false);

		_ = httpResponseMessage.EnsureSuccessStatusCode();
	}
}
