namespace AssociationRegistry.Public.Api.Verenigingen.Detail;

using Be.Vlaanderen.Basisregisters.AspNetCore.Mvc.Formatters.Json;
using Infrastructure.ConfigurationBindings;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Newtonsoft.Json;
using Schema.Detail;
using System.Text;

public class ResponseWriter : IResponseWriter
{
    private readonly AppSettings _appSettings;
    private readonly JsonSerializerSettings _serializerSettings;

    public ResponseWriter(AppSettings appSettings)
    {
        _appSettings = appSettings;
        _serializerSettings = JsonSerializerSettingsProvider.CreateSerializerSettings().ConfigureDefaultForApi();
    }

    public async Task Write(HttpResponse response, IAsyncEnumerable<PubliekVerenigingDetailDocument> data, CancellationToken cancellationToken)
    {
        await using var writer = new StreamWriter(response.Body, Encoding.UTF8);

        await foreach (var vereniging in data.WithCancellation(cancellationToken))
        {
            var mappedVereniging = PubliekVerenigingDetailMapper.MapDetailAll(vereniging, _appSettings);
            var json = JsonConvert.SerializeObject(mappedVereniging, _serializerSettings);
            await writer.WriteLineAsync(json);
        }
    }
}
