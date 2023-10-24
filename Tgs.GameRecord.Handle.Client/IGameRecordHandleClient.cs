using Tgs.GameRecord.Handle.Common.Protos;

namespace Tgs.GameRecord.Handle.Client;

public interface IGameRecordHandleClient
{
    /// <summary>
    /// Messages the asynchronous.
    /// </summary>
    /// <param name="gameProviderCode">The game provider code.</param>
    /// <param name="playerCode">The player code.</param>
    /// <param name="wagerState">State of the wager.</param>
    /// <param name="wagerType">Type of the wager.</param>
    /// <param name="freeGameState">State of the free game.</param>
    /// <param name="gambleState">State of the gamble.</param>
    /// <param name="cashoutState">State of the cashout.</param>
    /// <param name="resettleState">State of the resettle.</param>
    /// <param name="externalRoundId">The external round identifier.</param>
    /// <param name="externalWagerId">The external wager identifier.</param>
    /// <param name="externalGameId">The external game identifier.</param>
    /// <param name="externalCreatedOn">The external created on.</param>
    /// <param name="externalClosedOn">The external closed on.</param>
    /// <param name="externalPayoffOn">The external payoff on.</param>
    /// <param name="externalResettleOn">The external resettle on.</param>
    /// <param name="bet">The bet.</param>
    /// <param name="validBet">The valid bet.</param>
    /// <param name="settle">The settle.</param>
    /// <param name="commission">The commission.</param>
    /// <param name="turnover">The turnover.</param>
    /// <param name="winLoss">The win loss.</param>
    /// <param name="additionalData">The additional data.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public ValueTask MessageAsync(string gameProviderCode, string playerCode, WagerStates wagerState, WagerTypes wagerType, FreeGameStates freeGameState, GambleStates gambleState, CashoutStates cashoutState, ResettleStates resettleState, string externalRoundId, string externalWagerId, string externalGameId, DateTimeOffset externalCreatedOn, DateTimeOffset? externalClosedOn, DateTimeOffset? externalPayoffOn, DateTimeOffset? externalResettleOn, decimal bet, decimal validBet, decimal settle, decimal commission, decimal turnover, decimal winLoss, string additionalData, CancellationToken cancellationToken);

    /// <summary>
    /// Messages the asynchronous.
    /// </summary>
    /// <param name="gameProviderCode">The game provider code.</param>
    /// <param name="playerCode">The player code.</param>
    /// <param name="wagerState">State of the wager.</param>
    /// <param name="wagerType">Type of the wager.</param>
    /// <param name="freeGameState">State of the free game.</param>
    /// <param name="gambleState">State of the gamble.</param>
    /// <param name="cashoutState">State of the cashout.</param>
    /// <param name="resettleState">State of the resettle.</param>
    /// <param name="externalRoundId">The external round identifier.</param>
    /// <param name="externalWagerId">The external wager identifier.</param>
    /// <param name="externalGameId">The external game identifier.</param>
    /// <param name="externalCreatedOn">The external created on.</param>
    /// <param name="externalClosedOn">The external closed on.</param>
    /// <param name="externalResettleOn">The external resettle on.</param>
    /// <param name="bet">The bet.</param>
    /// <param name="validBet">The valid bet.</param>
    /// <param name="settle">The settle.</param>
    /// <param name="commission">The commission.</param>
    /// <param name="turnover">The turnover.</param>
    /// <param name="winLoss">The win loss.</param>
    /// <param name="additionalData">The additional data.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public ValueTask MessageAsync(string gameProviderCode, string playerCode, WagerStates wagerState, WagerTypes wagerType, FreeGameStates freeGameState, GambleStates gambleState, CashoutStates cashoutState, ResettleStates resettleState, string externalRoundId, string externalWagerId, string externalGameId, DateTimeOffset externalCreatedOn, DateTimeOffset? externalClosedOn, DateTimeOffset? externalResettleOn, decimal bet, decimal validBet, decimal settle, decimal commission, decimal turnover, decimal winLoss, string additionalData, CancellationToken cancellationToken);

    /// <summary>
    /// Messages the asynchronous.
    /// </summary>
    /// <param name="gameProviderCode">The game provider code.</param>
    /// <param name="playerCode">The player code.</param>
    /// <param name="wagerState">State of the wager.</param>
    /// <param name="wagerType">Type of the wager.</param>
    /// <param name="freeGameState">State of the free game.</param>
    /// <param name="gambleState">State of the gamble.</param>
    /// <param name="cashoutState">State of the cashout.</param>
    /// <param name="resettleState">State of the resettle.</param>
    /// <param name="externalRoundId">The external round identifier.</param>
    /// <param name="externalWagerId">The external wager identifier.</param>
    /// <param name="externalGameId">The external game identifier.</param>
    /// <param name="externalCreatedOn">The external created on.</param>
    /// <param name="externalClosedOn">The external closed on.</param>
    /// <param name="externalResettleOn">The external resettle on.</param>
    /// <param name="bet">The bet.</param>
    /// <param name="validBet">The valid bet.</param>
    /// <param name="settle">The settle.</param>
    /// <param name="commission">The commission.</param>
    /// <param name="turnover">The turnover.</param>
    /// <param name="winLoss">The win loss.</param>
    /// <param name="additionalData">The additional data.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public ValueTask MessageAsync(string gameProviderCode, string playerCode, WagerStates wagerState, WagerTypes wagerType, FreeGameStates freeGameState, GambleStates gambleState, CashoutStates cashoutState, string externalRoundId, string externalWagerId, string externalGameId, DateTimeOffset externalCreatedOn, DateTimeOffset? externalClosedOn, decimal bet, decimal validBet, decimal settle, decimal commission, decimal turnover, decimal winLoss, string additionalData, CancellationToken cancellationToken);
}
