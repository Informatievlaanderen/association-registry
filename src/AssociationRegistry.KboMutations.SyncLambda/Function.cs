using System.Text.Json.Serialization;
using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.Lambda.SQSEvents;

namespace AssociationRegistry.KboMutations.SyncLambda;

using Configuration;
using OpenTelemetry;
using Serilog;
using Services;
using Telemetry;

public class Function
{
    private static async Task Main()
    {
        var handler = FunctionHandler;
        await LambdaBootstrapBuilder.Create(handler, new SourceGeneratorLambdaJsonSerializer<LambdaFunctionJsonSerializerContext>())
            .Build()
            .RunAsync();
    }

    private static async Task FunctionHandler(SQSEvent @event, ILambdaContext context)
    {
        context.Logger.LogInformation("Function started.");
        var configurationManager = new ConfigurationManager();
        var configuration = configurationManager.Build();

        var telemetryManager = new TelemetryManager(context.Logger, configuration);
        var serviceFactory = new ServiceFactory(configuration, context.Logger, telemetryManager);

        try
        {
            context.Logger.LogInformation($"{@event.Records.Count} RECORDS RECEIVED INSIDE SQS EVENT");

            var services = await serviceFactory.CreateServicesAsync();

            await services.MessageProcessor.ProcessMessage(
                @event,
                services.LoggerFactory,
                services.RegistreerInschrijvingService,
                services.GeefVerenigingService,
                services.Repository,
                services.VertegenwoordigerPersoonsgegevensRepository,
                services.Notifier,
                CancellationToken.None);

            context.Logger.LogInformation($"{@event.Records.Count} RECORDS PROCESSED BY THE MESSAGE PROCESSOR");
        }
        catch (Exception e)
        {
            context.Logger.LogError(e, e.Message);
            await telemetryManager.FlushAsync(context);
            throw;
        }
        finally
        {
            await Log.CloseAndFlushAsync();
            await telemetryManager.FlushAsync(context);
        }
    }
}

[JsonSerializable(typeof(SQSEvent))]
public partial class LambdaFunctionJsonSerializerContext : JsonSerializerContext
{
}
