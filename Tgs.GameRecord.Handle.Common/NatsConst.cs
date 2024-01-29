namespace Tgs.GameRecord.Handle.Common;

public record NatsConst
{
    /*
     * game-gamerecord-*相關服務使用傳送到game-gamerecord-handle
     */
    public const string GameRecordWagerPublish = "game.gamerecord.wager.publish";
    /*
     * game-gamerecord-handle處理完時候進行推播
     */
    public const string GameRecordHandleDataPublish = "game.gamerecord.handle.data.publish";
    /*
     * game-gamewagers-rapidtest處理完時候進行推播
     */
    public const string GameWagerRapidTestDataPublish = "game.gamewager.rapidtest.data.publish";
    /*
     * game-gamewagers-unsettle處理完時候進行推播
     */
    public const string GameWagerTempUnsettleDataPublish = "game.gamewager.temp.unsettle.data.publish";
    /*
     * game-gamewagers-handle處理完時候進行推播
     */
    public const string GameWagerDataBaseDetailPublish = "game.gamewager.db.detail.publish";
    /*
     * game-gamewagers-handle處理完時候進行推播
     */
    public const string GameWagerDataBaseFirstPublish = "game.gamewager.db.first.publish";
    /*
     * game-gamewagers-handle處理完時候進行推播
     */
    public const string GameWagerDataBaseSettlePublish = "game.gamewager.db.settle.publish";
    /*
     * game-gamewagers-firsts處理完時候進行推播
     */
    public const string GameWagerSettleFirstDataPublish = "game.gamewager.settle.first.data.publish";
}
