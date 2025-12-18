namespace AssociationRegistry.KboMutations.SyncLambda;

using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.Lambda.SQSEvents;
using Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Text.Json.Serialization;
using Telemetry;

public class Function
{
    private static IHost? _host;

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

        // Initialize host on first invocation (Lambda warm start optimization)
        if (_host == null)
        {
            var configurationManager = new ConfigurationManager();
            var configuration = configurationManager.Build();
            _host = await HostConfiguration.CreateHost(context, configuration);
        }

        var telemetryManager = _host.Services.GetRequiredService<TelemetryManager>();

        try
        {
            context.Logger.LogInformation($"{@event.Records.Count} RECORDS RECEIVED INSIDE SQS EVENT");

            var messageProcessor = _host.Services.GetRequiredService<MessageProcessor>();
            await messageProcessor.ProcessMessage(@event, CancellationToken.None);

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
