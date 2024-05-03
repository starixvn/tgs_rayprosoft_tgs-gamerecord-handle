namespace Tgs.GameRecord.Handle.Core;
public record GameRecordHandleSettingsOptions
{
	public int ChannelExpiry { get; set; } = 900;
}
