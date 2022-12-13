namespace AssociationRegistry.OpenTelemetry.Extensions;

using System.Reflection;
using global::OpenTelemetry.Exporter;
using global::OpenTelemetry.Logs;
using global::OpenTelemetry.Metrics;
using global::OpenTelemetry.Resources;
using global::OpenTelemetry.Trace;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public static class ServiceCollectionExtensions
{
    public static void AddOpenTelemetry(this IServiceCollection services)
    {
        var executingAssembly = Assembly.GetExecutingAssembly();
        var serviceName = executingAssembly.FullName;
        var serviceVersion = "1.0.0";
        var assemblyVersion = executingAssembly.GetName().Version?.ToString() ?? "unknown";
        Action<ResourceBuilder> configureResource = r => r.AddService(
                serviceName, serviceVersion: assemblyVersion, serviceInstanceId: Environment.MachineName)
            .AddAttributes(
                new Dictionary<string, object>()
                {
                    ["deployment.environment"] =
                        Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.ToLowerInvariant() ?? "unknown",
                });

        services.AddOpenTelemetryTracing(
            b =>
            {
                b
                    .AddSource(serviceName)
                    .ConfigureResource(configureResource).AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation(
                        options =>
                        {
                            options.EnrichWithHttpRequest =
                                (activity, request) => activity.SetParentId(request.Headers["traceparent"]);
                            options.Filter = context => context.Request.Method != HttpMethods.Options;
                        })
                    // .AddSqlClientInstrumentation(options => { options.SetDbStatementForText = true; })
                    .AddOtlpExporter(
                        options =>
                        {
                            options.Protocol = OtlpExportProtocol.Grpc;
                            options.Endpoint = new Uri("http://localhost:4317");
                        });
                // .AddConsoleExporter();
            });

        services.AddLogging(
            builder =>
            {
                builder.ClearProviders();
                builder.AddOpenTelemetry(
                    options =>
                    {
                        options.ConfigureResource(configureResource);

                        options.IncludeScopes = true;
                        options.IncludeFormattedMessage = true;
                        options.ParseStateValues = true;

                        options.AddOtlpExporter(
                                exporter =>
                                {
                                    exporter.Protocol = OtlpExportProtocol.Grpc;
                                    exporter.Endpoint = new Uri("http://localhost:4317");
                                })
                            .AddConsoleExporter();
                    });
            });

        services.AddOpenTelemetryMetrics(
            options =>
            {
                options.ConfigureResource(configureResource)
                    .AddRuntimeInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation();

                options.AddOtlpExporter(
                    exporter =>
                    {
                        exporter.Protocol = OtlpExportProtocol.Grpc;
                        exporter.Endpoint = new Uri("http://localhost:4317");
                    });
                // options.AddConsoleExporter();
            });
    }
}
