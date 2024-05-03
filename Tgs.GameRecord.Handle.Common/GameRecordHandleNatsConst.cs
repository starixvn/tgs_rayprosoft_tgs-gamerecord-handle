namespace Tgs.GameRecord.Handle.Common;

public record GameRecordHandleNatsConst
{
	/*
     * game-gamerecord-*相關服務使用傳送到game-gamerecord-handle
     */
	public const string GameRecordWagerPublish = "game.gamerecord.wager.publish";
	public const string GameRecordHandleRequest = "game.gamerecord.handle.request";
	public const string GameRecordHandleExpiryRequest = "game.gamerecord.handle.expiry.request";
}
