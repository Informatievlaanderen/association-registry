namespace AssociationRegistry.Public.Api.Verenigingen.Detail;

using Amazon.S3;
using Amazon.S3.Model;
using Be.Vlaanderen.Basisregisters.AspNetCore.Mvc.Formatters.Json;
using Infrastructure.ConfigurationBindings;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Newtonsoft.Json;
using Schema.Constants;
using Schema.Detail;
using System.Text;
using PutObjectRequest = Amazon.S3.Model.PutObjectRequest;

public class ResponseWriter : IResponseWriter
{
    private readonly AppSettings _appSettings;
    private readonly JsonSerializerSettings _serializerSettings;
    private readonly IAmazonS3 _s3Client;

    public ResponseWriter(AppSettings appSettings, IAmazonS3 s3Client)
    {
        _appSettings = appSettings;
        _s3Client = s3Client;
        _serializerSettings = JsonSerializerSettingsProvider.CreateSerializerSettings().ConfigureDefaultForApi();
    }

    public async Task Write(
        HttpResponse response,
        IAsyncEnumerable<PubliekVerenigingDetailDocument> data,
        CancellationToken cancellationToken)
    {
        await using var writer = new StreamWriter(response.Body, Encoding.UTF8);

        await foreach (var vereniging in data.WithCancellation(cancellationToken))
        {
            if (IsTeVerwijderenVereniging(vereniging))
            {
                var teVerwijderenVereniging =
                    JsonConvert.SerializeObject(
                        new TeVerwijderenVereniging()
                        {
                            Vereniging = new TeVerwijderenVereniging.TeVerwijderenVerenigingData()
                            {
                                VCode = vereniging.VCode,
                                TeVerwijderen = true,
                                DeletedAt = vereniging.DatumLaatsteAanpassing,
                            }
                        },
                        _serializerSettings);

                await writer.WriteLineAsync(teVerwijderenVereniging);

                continue;
            }

            var mappedVereniging = PubliekVerenigingDetailMapper.MapDetailAll(vereniging, _appSettings);
            var json = JsonConvert.SerializeObject(mappedVereniging, _serializerSettings);
            await writer.WriteLineAsync(json);
        }
    }

    public async Task<string> WriteToS3(
        HttpResponse response,
        IAsyncEnumerable<PubliekVerenigingDetailDocument> data,
        CancellationToken cancellationToken)
    {
        using var memoryStream = new MemoryStream();
        await using var writer = new StreamWriter(memoryStream, Encoding.UTF8);

        await foreach (var vereniging in data.WithCancellation(cancellationToken))
        {
            if (IsTeVerwijderenVereniging(vereniging))
            {
                var teVerwijderenVereniging = JsonConvert.SerializeObject(
                    new TeVerwijderenVereniging()
                    {
                        Vereniging = new TeVerwijderenVereniging.TeVerwijderenVerenigingData()
                        {
                            VCode = vereniging.VCode,
                            TeVerwijderen = true,
                            DeletedAt = vereniging.DatumLaatsteAanpassing,
                        }
                    },
                    _serializerSettings);

                await writer.WriteLineAsync(teVerwijderenVereniging);
            }
            else
            {
                var mappedVereniging = PubliekVerenigingDetailMapper.MapDetailAll(vereniging, _appSettings);
                var json = JsonConvert.SerializeObject(mappedVereniging, _serializerSettings);
                await writer.WriteLineAsync(json);
            }
        }

        await writer.FlushAsync();
        memoryStream.Position = 0;

        var putRequest = new PutObjectRequest
        {
            BucketName = _appSettings.DetailAllS3.BucketName,
            Key = _appSettings.DetailAllS3.Key,
            InputStream = memoryStream,
            ContentType = "text/plain",
        };

        await _s3Client.PutObjectAsync(putRequest);

        var preSignedUrlRequest = new GetPreSignedUrlRequest
        {
            BucketName = _appSettings.DetailAllS3.BucketName,
            Key = _appSettings.DetailAllS3.Key,
            Expires = DateTime.UtcNow.Add(TimeSpan.FromMinutes(5)),
        };

        return await _s3Client.GetPreSignedURLAsync(preSignedUrlRequest);
    }

    private static bool IsTeVerwijderenVereniging(PubliekVerenigingDetailDocument vereniging)
        => vereniging.Deleted ||
           vereniging.Status == VerenigingStatus.Gestopt ||
           vereniging.IsUitgeschrevenUitPubliekeDatastroom is not null &&
           vereniging.IsUitgeschrevenUitPubliekeDatastroom.Value;

    public class TeVerwijderenVereniging
    {
        public TeVerwijderenVerenigingData Vereniging { get; set; }

        public class TeVerwijderenVerenigingData
        {
            public string VCode { get; set; }
            public bool TeVerwijderen { get; set; }
            public string DeletedAt { get; set; }
        }
    }
}
