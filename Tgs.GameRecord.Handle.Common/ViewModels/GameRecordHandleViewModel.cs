using System.ComponentModel;

using System.ComponentModel.DataAnnotations;

namespace Tgs.GameRecord.Handle.Common.ViewModels;
public record GameRecordHandleViewModel
{
	[Required]
	[DefaultValue(type: typeof(string), value: "LY")]
	public string GameProviderCode { get; init; } = default!;
	[Required]
	[DefaultValue(type: typeof(string), value: "4un4b9l34304")]
	public string PlayerCode { get; init; } = default!;
	[Required]
	[DefaultValue(type: typeof(string), value: "888-1722937124-92240583-21")]
	public string ExternalRoundId { get; init; } = default!;
	[Required]
	[DefaultValue(type: typeof(string), value: "888-1722937124-92240583-21")]
	public string ExternalBetId { get; init; } = default!;
	[Required]
	[DefaultValue(type: typeof(string), value: "910")]
	public string ExternalGameId { get; init; } = default!;
	[Required]
	[DefaultValue(type: typeof(DateTimeOffset), value: "2024-08-06T17:38:44.000+08:00")]
	public DateTimeOffset ExternalCreatedOn { get; init; }
	[Required]
	[DefaultValue(type: typeof(DateTimeOffset), value: "2024-08-06T17:39:03.000+08:00")]
	public DateTimeOffset ExternalClosedOn { get; init; }
	[Required]
	[DefaultValue(type: typeof(DateTimeOffset), value: "2024-08-06T17:39:03.000+08:00")]
	public DateTimeOffset ExternalPayoffOn { get; init; }
	[Required]
	[DefaultValue(type: typeof(DateTimeOffset), value: "1970-01-01T00:00:00.000+00:00")]
	public DateTimeOffset ExternalResettleOn { get; init; }
	[Required]
	[DefaultValue(type: typeof(string), value: "Win")]
	public Protos.BetState BetState { get; init; }
	[Required]
	[DefaultValue(type: typeof(string), value: "Normal")]
	public Protos.BetType BetType { get; init; }
	[Required]
	[DefaultValue(type: typeof(string), value: "None")]
	public Protos.FreeGameState FreeGameState { get; init; }
	[Required]
	[DefaultValue(type: typeof(string), value: "None")]
	public Protos.GambleState GambleState { get; init; }
	[Required]
	[DefaultValue(type: typeof(string), value: "None")]
	public Protos.CashoutState CashoutState { get; init; }
	[Required]
	[DefaultValue(type: typeof(string), value: "None")]
	public Protos.ResettleState ResettleState { get; init; }
	[Required]
	[DefaultValue(type: typeof(string), value: "None")]
	public Protos.JackpotState JackpotState { get; init; }
	[Required]
	[Range(minimum: 0, maximum: double.MaxValue)]
	[DefaultValue(typeof(decimal), "0")]
	public decimal Bet { get; init; }
	[Required]
	[Range(minimum: 0, maximum: double.MaxValue)]
	[DefaultValue(typeof(decimal), "0")]
	public decimal ValidBet { get; init; }
	[Required]
	[DefaultValue(typeof(decimal), "0")]
	public decimal Settle { get; init; }
	[Required]
	[DefaultValue(typeof(decimal), "0")]
	public decimal WinLoss { get; init; }
	[Required]
	[Range(minimum: 0, maximum: double.MaxValue)]
	[DefaultValue(typeof(decimal), "0")]
	public decimal Turnover { get; init; }
	[Range(minimum: 0, maximum: double.MaxValue)]
	[DefaultValue(typeof(decimal), "0")]
	public decimal Commission { get; init; }
	[Range(minimum: 0, maximum: double.MaxValue)]
	[DefaultValue(typeof(decimal), "0")]
	public decimal Contribute { get; init; }
	[Range(minimum: 0, maximum: double.MaxValue)]
	[DefaultValue(typeof(decimal), "0")]
	public decimal Jackpot { get; init; }
	[Required]
	[DefaultValue(typeof(decimal), "{}")]
	public string AdditionalData { get; init; } = default!;
}
