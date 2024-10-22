namespace AssociationRegistry.Public.Api.Verenigingen.Detail;

using Be.Vlaanderen.Basisregisters.AspNetCore.Mvc.Formatters.Json;
using Infrastructure.ConfigurationBindings;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Newtonsoft.Json;
using Schema.Constants;
using Schema.Detail;
using System.Text;

public class DetailAllWriter : IDetailAllWriter
{
    private readonly IS3Wrapper _s3Wrapper;
    private readonly AppSettings _appSettings;
    private readonly JsonSerializerSettings _serializerSettings;

    public DetailAllWriter(IS3Wrapper s3Wrapper, AppSettings appSettings)
    {
        _s3Wrapper = s3Wrapper;
        _appSettings = appSettings;
        _serializerSettings = JsonSerializerSettingsProvider.CreateSerializerSettings().ConfigureDefaultForApi();
    }

    public async Task WriteToS3Async(
        IAsyncEnumerable<PubliekVerenigingDetailDocument> data,
        CancellationToken cancellationToken)
    {
        await using var inputStream = new MemoryStream();
        await using var writer = new StreamWriter(inputStream, Encoding.UTF8);

        await foreach (var vereniging in data.WithCancellation(cancellationToken))
        {
            if (IsTeVerwijderenVereniging(vereniging))
            {
                var teVerwijderenVereniging =
                    JsonConvert.SerializeObject(
                        new TeVerwijderenVereniging
                        {
                            Vereniging = new TeVerwijderenVereniging.TeVerwijderenVerenigingData
                            {
                                VCode = vereniging.VCode,
                                TeVerwijderen = true,
                                DeletedAt = vereniging.DatumLaatsteAanpassing,
                            },
                        },
                        _serializerSettings);

                await writer.WriteLineAsync(teVerwijderenVereniging);

                continue;
            }

            var mappedVereniging = PubliekVerenigingDetailMapper.MapDetailAll(vereniging, _appSettings);
            var json = JsonConvert.SerializeObject(mappedVereniging, _serializerSettings);
            await writer.WriteLineAsync(json);
        }

        await writer.FlushAsync(cancellationToken);

        await _s3Wrapper.PutAsync(inputStream, cancellationToken);
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
