namespace AssociationRegistry.Public.Api.Verenigingen.Detail;

using Be.Vlaanderen.Basisregisters.AspNetCore.Mvc.Formatters.Json;
using Infrastructure.ConfigurationBindings;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Newtonsoft.Json;
using Schema.Constants;
using Schema.Detail;
using System.Text;

public class DetailAllStreamWriter : IDetailAllStreamWriter
{
    private readonly AppSettings _appSettings;
    private readonly JsonSerializerSettings _serializerSettings;

    public DetailAllStreamWriter(AppSettings appSettings)
    {
        _appSettings = appSettings;
        _serializerSettings = JsonSerializerSettingsProvider.CreateSerializerSettings().ConfigureDefaultForApi();
    }

    public async Task<MemoryStream> WriteAsync(IAsyncEnumerable<PubliekVerenigingDetailDocument> data, CancellationToken cancellationToken)
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
        inputStream.Position = 0;

        var ms = new MemoryStream();
        await inputStream.CopyToAsync(ms, cancellationToken);

        return ms;
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
