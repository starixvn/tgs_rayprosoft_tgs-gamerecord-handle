using System.Net.Mime;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NATS.Client.Core;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Tgs.GameRecord.Handle.Core;
using Tgs.GameRecord.Handle.Messages;
using Tgs.MessageQueue.Nats.Serializers;

var filterInstrumentations = new[] { "html", "ico", "png", "jpg" };

var builder = WebApplication.CreateBuilder(args);

builder.Logging
	.AddOpenTelemetry(options =>
	{
		options.IncludeFormattedMessage = true;
		options.ParseStateValues = true;
		_ = options.AddFluentdForwardExporter(fluentOptions =>
		{
			var nodeIp = builder.Configuration.GetValue<string>("Otel:NodeIp")!;
			fluentOptions.Host = nodeIp;
			fluentOptions.Tag = builder.Configuration.GetValue<string>("Otel:ServiceName")!;
			fluentOptions.UseMessagePack();
		});
	});

// Add services to the container.

builder.Services
	.AddRouting(options => options.LowercaseUrls = true)
	.AddControllers(options =>
	{
		options.Filters.Add(new ProducesAttribute(MediaTypeNames.Application.Json));
		options.Filters.Add(new ConsumesAttribute(MediaTypeNames.Application.Json));
	})
	.AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	foreach (var xmlFileName in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.xml"))
	{
		c.IncludeXmlComments(xmlFileName);
	}
});

builder.Services
	.AddOptions<PolicySettingsOptions>()
	.Configure((options) => builder.Configuration.GetSection("PolicySettings").Bind(options))
	.Services
	.AddOptions<GameRecordHandleSettingsOptions>()
	.Configure((options) => builder.Configuration.GetSection("GameRecordHandleSettings").Bind(options))
	.Services
	.AddMemoryCache()
	.AddGameCurrencyClient(httpClientBuilder => httpClientBuilder.ConfigureHttpClient((sp, http) =>
	{
		var config = sp.GetRequiredService<IConfiguration>();
		http.BaseAddress = config.GetValue<Uri>("GameRecordHandleConnectionStrings:GameCurrencyClientBaseUrl");
	}))
	.AddGameBetsClient(httpClientBuilder => httpClientBuilder.ConfigureHttpClient((sp, http) =>
	{
		var config = sp.GetRequiredService<IConfiguration>();
		http.BaseAddress = config.GetValue<Uri>("GameRecordHandleConnectionStrings:GameBetsClientBaseUrl");
	}))
	.AddGameBetsRapidTestWorkerClient(httpClientBuilder => httpClientBuilder.ConfigureHttpClient((sp, http) =>
	{
		var config = sp.GetRequiredService<IConfiguration>();
		http.BaseAddress = config.GetValue<Uri>("GameRecordHandleConnectionStrings:GameBetsRapidTestWorkerClientBaseUrl");
	}))
	.AddGameRecordHandleCore()
	.AddGameRecordHandleRepositories(
	(sp, dbBuilder) =>
	{
		var config = sp.GetRequiredService<IConfiguration>();
		_ = dbBuilder.UseNpgsql(config.GetConnectionString("Tgs"),
			builder => builder.EnableRetryOnFailure(
				maxRetryCount: 5,
				maxRetryDelay: TimeSpan.FromSeconds(10),
				errorCodesToAdd: null));
	});

builder.Services
	.AddKeyedSingleton("Nats", (sp, _) => new NatsConnection(NatsOpts.Default with
	{
		Url = builder.Configuration.GetConnectionString("Nats")!,
		SerializerRegistry = TgsNatsSerializerRegistration.Default
	}))
	.AddMessageQueue()
	.AddNatsMessageQueue(config => config
	.ConfigureResolveConnection(sp => sp.GetRequiredKeyedService<NatsConnection>("Nats"))
	.AddHandler<GameRecordHandleMessageHandler>(Tgs.GameRecord.Handle.Common.GameRecordHandleNatsConst.GameRecordHandleRequest, Tgs.GameRecord.Handle.Common.GameRecordHandleNatsConst.GameRecordHandleRequest)
	.AddHandler<GameRecordHandleExpiryTempMessageHandler>(Tgs.GameRecord.Handle.Common.GameRecordHandleNatsConst.GameRecordHandleRequest, string.Concat(Tgs.GameRecord.Handle.Common.GameRecordHandleNatsConst.GameRecordHandleExpiryRequest, ".", Environment.MachineName))
	.AddHandler<GameRecordHandleExpiryMessageHandler>(Tgs.GameRecord.Handle.Common.GameRecordHandleNatsConst.GameRecordHandleExpiryRequest, string.Concat(Tgs.GameRecord.Handle.Common.GameRecordHandleNatsConst.GameRecordHandleExpiryRequest, ".", Environment.MachineName)))
	.ClearAllExchanges()
	.AddNatsGlobPatternExchange(Tgs.GameRecord.Handle.Common.GameRecordHandleNatsConst.GameRecordHandleRequest)
	.AddNatsGlobPatternExchange(Tgs.GameRecord.Handle.Common.GameRecordHandleNatsConst.GameRecordHandleExpiryRequest);

builder.Services
	.AddOpenTelemetry()
	.ConfigureResource(_ => _.AddService(builder.Configuration.GetValue<string>("Otel:ServiceName")!))
	.WithTracing(builder => builder
		.SetResourceBuilder(ResourceBuilder.CreateDefault()
			.AddEnvironmentVariableDetector())
		.AddSource("Tgs.*")
		.AddSource("TGS.*")
		.AddSource("Valhalla.*")
		.AddAspNetCoreInstrumentation()
		.AddHttpClientInstrumentation()
		.AddGrpcClientInstrumentation()
		.AddGrpcCoreInstrumentation()
		.AddEntityFrameworkCoreInstrumentation(options => options.SetDbStatementForText = true)
		.AddOtlpExporter())
	.WithMetrics(meterBuiler => meterBuiler
	.SetResourceBuilder(ResourceBuilder.CreateDefault()
	.AddEnvironmentVariableDetector())
	.AddRuntimeInstrumentation()
	.AddAspNetCoreInstrumentation()
	.AddOtlpExporter(options => options.Endpoint = builder.Configuration.GetValue<Uri>("Otel:Collector")!))
	.Services
	.Configure<AspNetCoreTraceInstrumentationOptions>(options => options.Filter =
		httpContext => !httpContext.Request.Path.HasValue
			|| !httpContext.Request.Path.StartsWithSegments("/swagger")
			&& !httpContext.Request.Path.StartsWithSegments("/health")
			&& !httpContext.Request.Path.StartsWithSegments("/profiler/")
			&& !httpContext.Request.Path.StartsWithSegments("/hangfire")
			&& !httpContext.Request.Path.StartsWithSegments("/metrics")
			&& !filterInstrumentations.Any(ext => httpContext.Request.Path.Value.EndsWith($".{ext}")))
	.AddInterceptionService()
	.AddServiceUtiltiyInterceptors();

builder.Services.AddHealthChecks();

var app = builder.Build();

_ = app.UseSwagger();
_ = app.UseSwaggerUI();

_ = app.UseAuthorization();

_ = app.MapControllers();

app.MapHealthChecks("/health");
app.MapHealthChecks("/");

_ = app.UseRegisterAsync();

app.Run();
