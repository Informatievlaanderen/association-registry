using System.Diagnostics.Metrics;
using System.Text.Json.Serialization;
using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.Lambda.SQSEvents;
using Microsoft.Extensions.Logging;

namespace AssociationRegistry.KboMutations.SyncLambda;

using Configuration;
using Exceptions;
using Logging;
using OpenTelemetry;
using Serilog;
using Services;
using Telemetry;

public class Function
{
    private static readonly AssociationRegistry.OpenTelemetry.Metrics.ColdStartDetector ColdStartDetector = new();

    private static async Task Main()
    {
        var handler = FunctionHandler;

        await LambdaBootstrapBuilder
            .Create(handler, new SourceGeneratorLambdaJsonSerializer<LambdaFunctionJsonSerializerContext>())
            .Build()
            .RunAsync();
    }

    private static async Task FunctionHandler(SQSEvent @event, ILambdaContext context)
    {
        context.Logger.LogInformation("Function started");

        var coldStart = ColdStartDetector.IsColdStart();

        var configurationManager = new ConfigurationManager();
        var configuration = configurationManager.Build();

        var telemetryManager = new TelemetryManager(context.Logger, configuration);

        telemetryManager.Metrics.RecordLambdaInvocation("kbo_sync", coldStart);

        var serviceFactory = new ServiceFactory(configuration, context.Logger, telemetryManager);

        LambdaServices? services = null;
        Exception? caughtException = null;

        var logger = new SyncLogger(services, context);

        try
        {
            context.Logger.LogInformation($"{@event.Records.Count} RECORDS RECEIVED INSIDE SQS EVENT");

            services = await serviceFactory.CreateServicesAsync();

            logger.LogInformation("Processing {RecordCount} SQS messages", @event.Records.Count);

            await services.MessageProcessor.ProcessMessage(
                @event,
                services.KboSyncHandler,
                services.KszSyncHandler,
                services.Repository,
                services.QueryService,
                CancellationToken.None
            );

            telemetryManager.Metrics.RecordFilesProcessed(@event.Records.Count);

            logger.LogInformation("Successfully processed {RecordCount} records", @event.Records.Count);
        }
        catch (KszSyncException e)
        {
            caughtException = e;

            logger.LogException(
                e,
                "VCode: '{VCode}', VertegenwoordigerId: '{VertegenwoordigerId}' \n KSZ sync lambda failed with error: {ErrorMessage}",
                e.VCode,
                e.VertegenwoordigerId,
                e.Message
            );
        }
        catch (KboSyncException e)
        {
            caughtException = e;

            logger.LogException(
                e,
                "VCode: '{VCode}', KboNummer: '{KboNummer}' \n KBO sync lambda failed with error: {ErrorMessage}",
                e.VCode,
                e.KboNummer,
                e.Message
            );
        }
        catch (Exception e)
        {
            caughtException = e;

            logger.LogException(e, "KBO/KSZ sync lambda failed with error: {ErrorMessage}", e.Message);
        }
        finally
        {
            logger.LogInformation("Kbo/ksz sync lambda finished");

            // Flush Serilog logs first
            await Log.CloseAndFlushAsync();

            // Dispose LoggerFactory to flush OpenTelemetry logs
            if (services != null)
            {
                context.Logger.LogInformation("Disposing LoggerFactory to flush logs");
                services.LoggerFactory.Dispose();
                context.Logger.LogInformation("LoggerFactory disposed");
            }

            // Flush metrics and traces
            await telemetryManager.FlushAsync(context);

            context.Logger.LogInformation("All telemetry flushed");
        }

        // Re-throw after all cleanup is complete
        if (caughtException != null)
        {
            context.Logger.LogInformation("Re-throwing exception after flush");

            throw caughtException;
        }
    }
}

[JsonSerializable(typeof(SQSEvent))]
public partial class LambdaFunctionJsonSerializerContext : JsonSerializerContext { }
