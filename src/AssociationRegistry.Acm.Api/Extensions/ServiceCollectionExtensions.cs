﻿namespace AssociationRegistry.Acm.Api.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Caches;
using S3;
using Be.Vlaanderen.Basisregisters.BlobStore.Aws;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

public static class ServiceCollectionExtensions
{
    public static void AddBlobClients(this IServiceCollection services, S3BlobClientOptions s3Options) =>
        services
            .AddSingleton(sp => new VerenigingenBlobClient(
                s3Options.Buckets[WellknownBuckets.Verenigingen.Name],
                bucketName => new S3BlobClient(sp.GetRequiredService<AmazonS3Client>(), bucketName),
                sp.GetRequiredService<ILogger<VerenigingenBlobClient>>()
            ));

    public static void AddS3(this IServiceCollection services, IConfiguration configuration)
    {
        // Use MINIO
        if (configuration.GetValue<string>("MINIO_SERVER") != null)
        {
            if (configuration.GetValue<string>("MINIO_ROOT_USER") == null)
            {
                throw new InvalidOperationException("The MINIO_ROOT_USER configuration variable was not set.");
            }

            if (configuration.GetValue<string>("MINIO_ROOT_PASSWORD") == null)
            {
                throw new InvalidOperationException("The MINIO_ROOT_PASSWORD configuration variable was not set.");
            }

            services.AddSingleton(new AmazonS3Client(
                    new BasicAWSCredentials(
                        configuration.GetValue<string>("MINIO_ROOT_USER"),
                        configuration.GetValue<string>("MINIO_ROOT_PASSWORD")),
                    new AmazonS3Config
                    {
                        RegionEndpoint = RegionEndpoint.USEast1, // minio's default region
                        ServiceURL = configuration.GetValue<string>("MINIO_SERVER"),
                        ForcePathStyle = true,
                    }
                )
            );
        }
        else // Use AWS
        {
            services.AddSingleton(new AmazonS3Client());
        }
    }

    public static void AddDataCache(this IServiceCollection services)
        => services.AddSingleton<IVerenigingenRepository>(
            sp =>
                VerenigingenRepository.Load(sp.GetRequiredService<VerenigingenBlobClient>()).GetAwaiter().GetResult());

    public static void AddOpenTelemetry(this IServiceCollection services)
    {
        var assembly = typeof(Startup).Assembly;
        var serviceName = assembly.FullName;
        var serviceVersion = "1.0.0";
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
