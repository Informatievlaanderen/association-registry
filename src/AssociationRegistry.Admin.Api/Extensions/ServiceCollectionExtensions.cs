namespace AssociationRegistry.Admin.Api.Extentions;

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

public static class ServiceCollectionExtensions
{
    public static void AddOpenTelemetry(this IServiceCollection services)
    {
        var assembly = typeof(Startup).Assembly;
        var serviceName = assembly.FullName!;
        var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown";
        Action<ResourceBuilder> configureResource = r => r.AddService(
                serviceName,
                serviceVersion: assemblyVersion,
                serviceInstanceId: Environment.MachineName)
            .AddAttributes(
                new Dictionary<string, object>
                {
                    ["deployment.environment"] =
                        Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.ToLowerInvariant() ?? "unknown",
                });

        services.AddOpenTelemetryTracing(
            builder => builder
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
                    }));

        services.AddLogging(
            builder => builder
                .ClearProviders()
                .AddOpenTelemetry(
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
                            });
                    }));

        services.AddOpenTelemetryMetrics(
            options => options
                .ConfigureResource(configureResource)
                .AddRuntimeInstrumentation()
                .AddHttpClientInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddOtlpExporter(
                    exporter =>
                    {
                        exporter.Protocol = OtlpExportProtocol.Grpc;
                        exporter.Endpoint = new Uri("http://localhost:4317");
                    }));
    }
}
