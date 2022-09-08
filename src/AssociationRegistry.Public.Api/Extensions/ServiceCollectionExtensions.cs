﻿using System;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using AssociationRegistry.Public.Api.Caches;
using AssociationRegistry.Public.Api.S3;
using Be.Vlaanderen.Basisregisters.BlobStore.Aws;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AssociationRegistry.Public.Api.Extensions;

using System.Collections.Immutable;
using System.IO;
using ListVerenigingen;
using Newtonsoft.Json;
using IVerenigingenRepository = Caches.IVerenigingenRepository;

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

    public static void AddDataCache(this IServiceCollection services) =>
        services.AddSingleton<IVerenigingenRepository>(sp =>
            VerenigingenRepository.Load(sp.GetRequiredService<VerenigingenBlobClient>()).GetAwaiter().GetResult());

    public static void AddJsonLdContexts(this IServiceCollection services)
    {
        services.AddSingleton<ListVerenigingContext>(sp =>
        {
            var blobObject = sp.GetRequiredService<VerenigingenBlobClient>().GetBlobAsync(WellknownBuckets.Verenigingen.Blobs.ListVerenigingenContext).GetAwaiter().GetResult();
            var blobStream = blobObject.OpenAsync().GetAwaiter().GetResult();
            using var streamReader = new StreamReader(blobStream);
            var json = streamReader.ReadToEndAsync().GetAwaiter().GetResult();
            var deserializeObject = JsonConvert.DeserializeObject<ListVerenigingContext>(json);
            return deserializeObject ??
                   throw new InvalidOperationException("Could not load ListVerenigingContext");
        });
        services.AddSingleton<IVerenigingenRepository>(sp =>
            VerenigingenRepository.Load(sp.GetRequiredService<VerenigingenBlobClient>()).GetAwaiter().GetResult());
    }
}
