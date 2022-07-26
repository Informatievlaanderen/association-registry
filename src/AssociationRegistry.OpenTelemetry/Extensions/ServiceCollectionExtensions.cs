﻿namespace AssociationRegistry.OpenTelemetry.Extensions;

using System.Reflection;
using global::OpenTelemetry.Exporter;
using global::OpenTelemetry.Logs;
using global::OpenTelemetry.Metrics;
using global::OpenTelemetry.Resources;
using global::OpenTelemetry.Trace;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;

public static class ServiceCollectionExtensions
{
    public static void AddOpenTelemetry(this IServiceCollection services)
    {
        var executingAssembly = Assembly.GetEntryAssembly()!;
        var serviceName = executingAssembly.FullName!;
        var assemblyVersion = executingAssembly.GetName().Version?.ToString() ?? "unknown";
        var collectorUrl = Environment.GetEnvironmentVariable("COLLECTOR_URL") ?? "http://localhost:4317";

        Action<ResourceBuilder> configureResource = r => r
            .AddService(
                serviceName,
                serviceVersion: assemblyVersion,
                serviceInstanceId: Environment.MachineName)
            .AddAttributes(
                new Dictionary<string, object>
                {
                    ["deployment.environment"] =
                        Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.ToLowerInvariant()
                        ?? "unknown",
                });

        services.AddOpenTelemetryTracing(
            builder =>
                builder
                    .AddSource(serviceName)
                    .ConfigureResource(configureResource).AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation(
                        options =>
                        {
                            options.EnrichWithHttpRequest =
                                (activity, request) => activity.SetParentId(request.Headers["traceparent"]);
                            options.Filter = context => context.Request.Method != HttpMethods.Options;
                        })
                    .AddNpgsql()
                    .AddOtlpExporter(
                        options =>
                        {
                            options.Protocol = OtlpExportProtocol.Grpc;
                            options.Endpoint = new Uri(collectorUrl);
                        }));

        services.AddLogging(
            builder =>
                builder
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
                                    exporter.Endpoint = new Uri(collectorUrl);
                                });
                        }));

        services.AddOpenTelemetryMetrics(
            options =>
                options
                    .ConfigureResource(configureResource)
                    .AddRuntimeInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddOtlpExporter(
                        exporter =>
                        {
                            exporter.Protocol = OtlpExportProtocol.Grpc;
                            exporter.Endpoint = new Uri(collectorUrl);
                        }));
    }
}
