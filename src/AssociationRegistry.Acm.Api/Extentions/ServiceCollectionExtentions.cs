using System;
using System.Collections.Generic;
using System.IO;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using AssociationRegistry.Acm.Api.Caches;
using AssociationRegistry.Acm.Api.S3;
using Be.Vlaanderen.Basisregisters.BlobStore;
using Be.Vlaanderen.Basisregisters.BlobStore.Aws;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace AssociationRegistry.Acm.Api.Extentions;

public static class ServiceCollectionExtentions
{
    public static IServiceCollection AddBlobClients(this IServiceCollection services, S3BlobClientOptions s3Options) =>
        services
            .AddSingleton(sp =>
            {
                var verenigingenBlobClient = new VerenigingenBlobClient(new S3BlobClient(
                    sp.GetRequiredService<AmazonS3Client>(),
                    s3Options.Buckets[WellknownBuckets.VerenigingenBucket]
                ));
                return verenigingenBlobClient;
            });

    public static IServiceCollection AddS3(this IServiceCollection services, IConfiguration configuration)
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
            if (configuration.GetValue<string>("AWS_ACCESS_KEY_ID") == null)
            {
                throw new InvalidOperationException("The AWS_ACCESS_KEY_ID configuration variable was not set.");
            }

            if (configuration.GetValue<string>("AWS_SECRET_ACCESS_KEY") == null)
            {
                throw new InvalidOperationException("The AWS_SECRET_ACCESS_KEY configuration variable was not set.");
            }

            services.AddSingleton(new AmazonS3Client(
                    new BasicAWSCredentials(
                        configuration.GetValue<string>("AWS_ACCESS_KEY_ID"),
                        configuration.GetValue<string>("AWS_SECRET_ACCESS_KEY"))
                )
            );
        }
        return services;
    }

    public static IServiceCollection AddDataCache(this IServiceCollection services)
    {
        services.AddSingleton(sp =>
        {
            var verenigingenBlobClient = sp.GetRequiredService<VerenigingenBlobClient>();
            var blobObject = verenigingenBlobClient.GetBlobAsync(new BlobName("data.json"))
                .GetAwaiter().GetResult();
            var blobstream = blobObject.OpenAsync().GetAwaiter().GetResult();
            var json = new StreamReader(blobstream).ReadToEnd();
            var jsonDictionary = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(json);
            Data.TryParse(jsonDictionary!, out var verenigingen);
            return new Data()
            {
                Verenigingen = verenigingen,
            };
        });
        return services;
    }
}
